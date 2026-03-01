using System.Text.Json;

namespace SuscripcionesYContratos.Infraestructura.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; set; }

    public string EventName { get; set; } = default!;
    public DateTime OccurredOnUtc { get; set; }
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;

    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }

    public static OutboxMessage From<T>(T message, DateTime? occurredOnUtc = null)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredOnUtc = occurredOnUtc ?? DateTime.UtcNow,
            Type = typeof(T).FullName ?? typeof(T).Name,
            Payload = JsonSerializer.Serialize(message)
        };
    }
}