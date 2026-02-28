using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SuscripcionesYContratos.Infraestructura.Mensajeria;
using SuscripcionesYContratos.Infraestructura.Persistencia.ModeloDominio;

namespace SuscripcionesYContratos.Infraestructura.Outbox;

internal sealed class OutboxProcessorBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorBackgroundService> _logger;
    private readonly RabbitMqOptions _rabbitOptions;

    public OutboxProcessorBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessorBackgroundService> logger,
        IOptions<RabbitMqOptions> rabbitOptions)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _rabbitOptions = rabbitOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromSeconds(Math.Max(1, _rabbitOptions.OutboxPublishIntervalSeconds));
        var batchSize = Math.Max(1, _rabbitOptions.OutboxBatchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<DomainDbContext>();
                var publisher = scope.ServiceProvider.GetRequiredService<IRabbitMqPublisher>();

                var pending = await db.OutboxMessages
                    .Where(x => x.ProcessedOnUtc == null)
                    .OrderBy(x => x.OccurredOnUtc)
                    .Take(batchSize)
                    .ToListAsync(stoppingToken);

                if (pending.Count == 0)
                {
                    await Task.Delay(interval, stoppingToken);
                    continue;
                }

                foreach (var msg in pending)
                {
                    try
                    {
                        var routingKey = !string.IsNullOrWhiteSpace(_rabbitOptions.OutputRoutingKey)
                            ? _rabbitOptions.OutputRoutingKey
                            : msg.Type.Replace('.', '_');

                        await publisher.PublishAsync(
                            exchange: _rabbitOptions.Exchange,
                            routingKey: routingKey,
                            body: msg.Payload,
                            ct: stoppingToken);

                        msg.ProcessedOnUtc = DateTime.UtcNow;
                        msg.Error = null;
                    }
                    catch (Exception ex)
                    {
                        msg.Error = ex.ToString();
                        _logger.LogError(ex, "Error publicando OutboxMessage {OutboxMessageId}", msg.Id);
                    }
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando Outbox");
            }

            await Task.Delay(interval, stoppingToken);
        }
    }
}