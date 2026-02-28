using Joseco.DDD.Core.Results;
using MediatR;
using SuscripcionesYContratos.Aplicacion.Suscripciones.DTO;
using SuscripcionesYContratos.Dominio.Suscripcion;

namespace SuscripcionesYContratos.Aplicacion.Suscripciones.ObtenerSuscripcion;

internal sealed class ObtenerSuscripcionHandler : IRequestHandler<ObtenerSuscripcionQuery, Result<SuscripcionDto>>
{
    private readonly ISuscripcionesRepo _repo;

    public ObtenerSuscripcionHandler(ISuscripcionesRepo repo)
    {
        _repo = repo;
    }

    public async Task<Result<SuscripcionDto>> Handle(ObtenerSuscripcionQuery request, CancellationToken cancellationToken)
    {
        var suscripcion = await _repo.GetByIdAsync(request.suscripcionId, readOnly: true);

        if (suscripcion is null)
            return Result.Failure<SuscripcionDto>(SuscripcionError.SuscripcionNoEncontrada);

        var dto = new SuscripcionDto
        {
            Id = suscripcion.Id,
            nombre = suscripcion.nombre,
            descripcion = suscripcion.descripcion,
            cantidadDias = suscripcion.cantidadDias,
            precioDia = suscripcion.precioDia,
            updateAt = suscripcion.updateAt
        };

        return Result.Success(dto);
    }
}