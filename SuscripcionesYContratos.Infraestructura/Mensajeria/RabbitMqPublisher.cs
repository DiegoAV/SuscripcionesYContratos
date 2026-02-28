using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace SuscripcionesYContratos.Infraestructura.Mensajeria;

public interface IRabbitMqPublisher
{
    Task PublishAsync(string exchange, string routingKey, string body, CancellationToken ct);
}

internal sealed class RabbitMqPublisher : IRabbitMqPublisher, IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly IConnection _connection;

    public RabbitMqPublisher(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection("SuscripcionesYContratos-Publisher");
    }

    public Task PublishAsync(string exchange, string routingKey, string body, CancellationToken ct)
    {
        using var channel = _connection.CreateModel();

        channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, durable: true, autoDelete: false);

        var props = channel.CreateBasicProperties();
        props.Persistent = true;
        props.ContentType = "application/json";
        props.DeliveryMode = 2;

        var bytes = Encoding.UTF8.GetBytes(body);
        channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: props, body: bytes);

        return Task.CompletedTask;
    }

    public void Dispose() => _connection.Dispose();
}