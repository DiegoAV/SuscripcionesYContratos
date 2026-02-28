using Joseco.DDD.Core.Results;
using MediatR;

namespace SuscripcionesYContratos.Aplicacion.Suscripciones.ActualizarSuscripcion;

public sealed record ActualizarSuscripcionCommand(
    Guid suscripcionId,
    string? nombre,
    string? descripcion,
    int? cantidadDias,
    decimal? precioDia
) : IRequest<Result<Guid>>;