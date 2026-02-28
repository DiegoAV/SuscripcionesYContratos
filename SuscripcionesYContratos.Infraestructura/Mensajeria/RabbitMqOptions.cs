namespace SuscripcionesYContratos.Infraestructura.Mensajeria;

public sealed class RabbitMqOptions
{
    public string HostName { get; init; } = "154.38.180.80";
    public int Port { get; init; } = 5672;
    public string UserName { get; init; } = "admin";
    public string Password { get; init; } = "rabbit_mq";
    public string VirtualHost { get; init; } = "/";
    public string Exchange { get; init; } = "outbox.events";
    //public string InputQueueName { get; set; } = "produccion.paquete-despacho-creado ";
    //public string InputRoutingKey { get; set; } = "produccion.paquete-despacho-creado";
    public string OutputRoutingKey { get; set; } = "";
    public bool DeclareTopology { get; set; } = false;
    public int ReconnectDelaySeconds { get; set; } = 10;
    public ushort PrefetchCount { get; set; } = 10;
    public int OutboxBatchSize { get; set; } = 50;
    public int OutboxPublishIntervalSeconds { get; set; } = 5;
}