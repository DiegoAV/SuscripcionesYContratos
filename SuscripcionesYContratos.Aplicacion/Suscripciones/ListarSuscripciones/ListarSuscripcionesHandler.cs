using Joseco.DDD.Core.Results;
using MediatR;
using SuscripcionesYContratos.Aplicacion.Suscripciones.DTO;
using SuscripcionesYContratos.Dominio.Suscripcion;

namespace SuscripcionesYContratos.Aplicacion.Suscripciones.ListarSuscripciones;

internal sealed class ListarSuscripcionesHandler : IRequestHandler<ListarSuscripcionesQuery, Result<IReadOnlyList<SuscripcionDto>>>
{
    private readonly ISuscripcionesRepo _repo;

    public ListarSuscripcionesHandler(ISuscripcionesRepo repo)
    {
        _repo = repo;
    }

    public async Task<Result<IReadOnlyList<SuscripcionDto>>> Handle(ListarSuscripcionesQuery request, CancellationToken cancellationToken)
    {
        var items = await _repo.ListAsync(readOnly: true, cancellationToken);

        var dto = items
            .Select(x => new SuscripcionDto
            {
                Id = x.Id,
                nombre = x.nombre,
                descripcion = x.descripcion,
                cantidadDias = x.cantidadDias,
                precioDia = x.precioDia,
                updateAt = x.updateAt
            })
            .ToList()
            .AsReadOnly();

        return Result.Success<IReadOnlyList<SuscripcionDto>>(dto);
    }
}