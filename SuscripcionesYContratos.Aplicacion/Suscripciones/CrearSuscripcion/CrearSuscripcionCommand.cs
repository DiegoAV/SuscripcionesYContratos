using Joseco.DDD.Core.Results;
using MediatR;
using System;

namespace SuscripcionesYContratos.Aplicacion.Suscripciones.CrearSuscripcion
{
    public record CrearSuscripcionCommand(
        string nombre,
        string descripcion,
        int cantidadDias,
        decimal precioDia
    ) : IRequest<Result<Guid>>;

    public sealed record SuscripcionDto(
        Guid Id,
        string nombre,
        string descripcion,
        int cantidadDias,
        decimal precioDia,
        DateTime? updateAt);
}
