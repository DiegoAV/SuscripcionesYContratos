using Joseco.DDD.Core.Abstractions;

namespace SuscripcionesYContratos.Dominio.Entregas.Eventos;

public sealed record CalendarioEntregaDomainEvent(
    Guid entregaId,
    Guid contratoId,
    DateOnly fecha,
    TimeOnly hora,
    int estado,
    DateTime occurredOnUtc) : DomainEvent;