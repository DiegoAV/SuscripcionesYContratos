using System;

namespace SuscripcionesYContratos.Aplicacion.CalendarioEntrega.ListarCalendarioEntrega;

public sealed record CalendarioEntregaDto(
    Guid Id,
    Guid contratoId,
    DateOnly fecha,
    TimeOnly hora,
    int estado,
    DateTime? updateAt);