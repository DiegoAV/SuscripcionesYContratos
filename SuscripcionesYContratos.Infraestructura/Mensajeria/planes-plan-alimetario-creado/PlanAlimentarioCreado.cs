using Joseco.DDD.Core.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SuscripcionesYContratos.Aplicacion.Contratos.CrearContrato;
using SuscripcionesYContratos.Aplicacion.Suscripciones.ObtenerSuscripcion;
using SuscripcionesYContratos.Dominio.Suscripcion;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SuscripcionesYContratos.Infraestructura.Mensajeria.planes_plan_alimetario_creado
{
    internal sealed class PlanAlimentarioCreado : BackgroundService
    {
        private static readonly JsonSerializerOptions PayloadJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PlanAlimentarioCreado> _logger;
        private readonly RabbitMqOptions _options;

        private IConnection? _connection;
        private IModel? _channel;
        public PlanAlimentarioCreado(IServiceScopeFactory scopeFactory, ILogger<PlanAlimentarioCreado> logger, IOptions<RabbitMqOptions> options)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _options = options.Value;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return RunConsumerLoopAsync(stoppingToken);
        }

        private async Task RunConsumerLoopAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    CreateConnection();
                    var consumer = new AsyncEventingBasicConsumer(_channel);
                    consumer.Received += async (_, eventArgs) =>
                    {
                        await HandleMessageAsync(eventArgs, stoppingToken);
                    };

                    _channel!.BasicConsume(
                       queue: _options.InputQueueName,
                       autoAck: false,
                       consumer: consumer);

                    _logger.LogInformation("Consumer de paquetes iniciado sobre la cola {Queue}", _options.InputQueueName);

                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // Graceful shutdown
                    _logger.LogInformation("Consumer de paquetes detenido por solicitud de cancelación.");
                    break;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error iniciando/ejecutando consumer RabbitMQ. Reintentando en {DelaySeconds}s", _options.ReconnectDelaySeconds);
                    DisposeConnection();
                    await Task.Delay(TimeSpan.FromSeconds(Math.Max(1, _options.ReconnectDelaySeconds)), stoppingToken);
                }
            }
        }

        private void DisposeConnection()
        {
            try
            {
                _channel?.Dispose();
                _connection?.Dispose();
            }
            finally
            {
                _channel = null;
                _connection = null;
            }
        }

        private async Task HandleMessageAsync(BasicDeliverEventArgs eventArgs, CancellationToken stoppingToken)
        {
            if (_channel is null)
            {
                return;
            }

            var raw = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            _logger.LogInformation("Mensaje recibido en {Queue}. Body: {Body}", _options.InputQueueName, raw);
            var payload = null as PlanAlimentarioPayload;

            try
            {
                payload = ExtractPayload(raw);
            }
            catch (JsonException)
            {
                _logger.LogWarning("Mensaje con formato JSON inválido en {Queue}. Body: {Body}", _options.InputQueueName, raw);
                _channel.BasicAck(eventArgs.DeliveryTag, false);
                return;
            }


            try
            {
                //    var raw = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                // var payload = JsonSerializer.Deserialize<PlanAlimentarioPayload>(raw, PayloadJsonOptions);
                //    var payload = ExtractPayload(raw);


                if (payload is null)
                {
                    _logger.LogWarning("Mensaje inválido en {Queue}. Body: {Body}", _options.InputQueueName, raw);
                    _channel.BasicAck(eventArgs.DeliveryTag, false);
                    return;
                }

                if (!IsValidPayload(payload, out var validationMessage))
                {
                    _logger.LogWarning(
                        "Mensaje descartado en {Queue}. Motivo: {Reason}. Body: {Body}",
                        _options.InputQueueName,
                        validationMessage,
                        raw);

                    _channel.BasicAck(eventArgs.DeliveryTag, false);
                    return;
                }

                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                /* Crear Contrato */
                var suscripcion = await mediator.Send(new ObtenerSuscripcionQuery(payload.idSubscription), stoppingToken);

                if (suscripcion is null)
                    return;

                if (suscripcion.Value.cantidadDias <= 0)
                    return;

                var incluyeFinDeSemana = false; // Asumimos que el plan alimentario incluye fines de semana, pero esto podría venir en el payload si es necesario

                var command = new CrearContratoCommand(
                    pacienteId: payload.idPatient,
                    suscripcionId: payload.idSubscription,
                    planId: payload.id,
                    hora: new TimeOnly(6, 30, 0),
                    inicio: payload.starDate,
                    incluyeFinDeSemana: incluyeFinDeSemana,
                    politicaCancelacionDias: 2
                );
                var result = await mediator.Send(command, stoppingToken);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Contrato Creado Existosamente {Id} para el Paciente {pacienteId}",
                        payload.id,
                        payload.idSubscription);
                    _channel.BasicAck(eventArgs.DeliveryTag, false);
                    return;
                }

                if (IsNonRetryableError(result.Error.Type))
                {
                    _logger.LogWarning(
                        "Mensaje descartado para paquete {PackageId}. Error no reintentable: {Code} - {Message}",
                        payload.id,
                        result.Error.Code,
                        result.Error.Description);

                    _channel.BasicAck(eventArgs.DeliveryTag, false);
                    return;
                }

                _logger.LogError(
                    "Error Creando el Contrato para el plan {Id}. Error: {Code} - {Message}",
                    payload.id,
                    result.Error.Code,
                    result.Error.Description);

                _channel.BasicNack(eventArgs.DeliveryTag, false, requeue: true);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error procesando mensaje de creación de paquete.");
                _channel.BasicNack(eventArgs.DeliveryTag, false, requeue: true);
            }
        }

        private bool IsNonRetryableError(ErrorType errorType)
        {
            return errorType is ErrorType.Validation or ErrorType.NotFound or ErrorType.Conflict;
        }

        private bool IsValidPayload(PlanAlimentarioPayload payload, out object validationMessage)
        {
            if (payload.id == Guid.Empty)
            {
                validationMessage = "id no puede ser Guid.Empty";
                return false;
            }

            if (payload.idSubscription == Guid.Empty)
            {
                validationMessage = "idSuscription no puede ser Guid.Empty";
                return false;
            }

            if (payload.idPatient == Guid.Empty)
            {
                validationMessage = "idPatient no puede ser Guid.Empty";
                return false;
            }

            validationMessage = string.Empty;
            return true;
        }

        private void CreateConnection()
        {
            DisposeConnection();

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.BasicQos(0, _options.PrefetchCount, false);

            if (_options.DeclareTopology)
            {
                _channel.ExchangeDeclare(_options.Exchange, ExchangeType.Topic, durable: true);
                _channel.QueueDeclare(_options.InputQueueName, durable: true, exclusive: false, autoDelete: false);
                _channel.QueueBind(_options.InputQueueName, _options.Exchange, _options.InputRoutingKey);
            }
        }

        private sealed class PlanAlimentarioPayload
        {
            public Guid id { get; set; }
            public Guid idNutricionist { get; set; }
            public Guid idPatient { get; set; }
            public Guid idSubscription { get; set; }  //idSubscription
            public int totalDays { get; set; }
            public DateOnly starDate { get; set; }
            public DateOnly endDate { get; set; }
            public int totalCalories { get; set; }
        }

        private static PlanAlimentarioPayload? ExtractPayload(string raw)
        {
            using var document = JsonDocument.Parse(raw);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (!string.Equals(property.Name, "payload", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                return property.Value.ValueKind == JsonValueKind.Object
                    ? JsonSerializer.Deserialize<PlanAlimentarioPayload>(property.Value.GetRawText(), PayloadJsonOptions)
                    : null;
            }

            return JsonSerializer.Deserialize<PlanAlimentarioPayload>(document.RootElement.GetRawText(), PayloadJsonOptions);
        }
    }
}
