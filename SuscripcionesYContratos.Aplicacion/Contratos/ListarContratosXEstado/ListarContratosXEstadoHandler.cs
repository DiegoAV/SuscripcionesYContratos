using Joseco.DDD.Core.Results;
using MediatR;
using SuscripcionesYContratos.Aplicacion.Contratos.DTO;
using SuscripcionesYContratos.Dominio.Contrato;

namespace SuscripcionesYContratos.Aplicacion.Contratos.ListarContratosXEstado;

internal sealed class ListarContratosXEstadoHandler : IRequestHandler<ListarContratosXEstadoQuery, Result<IReadOnlyList<ContratoDto>>>
{
    private readonly IContratosRepo _repo;

    public ListarContratosXEstadoHandler(IContratosRepo repo)
    {
        _repo = repo;
    }

    public async Task<Result<IReadOnlyList<ContratoDto>>> Handle(ListarContratosXEstadoQuery request, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(typeof(ContratoEstado), request.estado))
            return Result.Failure<IReadOnlyList<ContratoDto>>(ContratoError.ContratoInvalido);

        var estado = (ContratoEstado)request.estado;

        var items = await _repo.ListByEstadoAsync(estado, readOnly: true, cancellationToken);

        var dto = items
            .Select(x => new ContratoDto
            {
                Id = x.Id,
                pacienteId = x.pacienteId,
                suscripcionId = x.suscripcionId,
                planId = x.planId,
                hora = x.hora,
                inicio = x.inicio,
                fin = x.fin,
                incluyeFinDeSemana = x.incluyeFinDeSemana,
                cantidadEntregas = x.cantidadEntregas,
                precioTotal = x.precioTotal,
                estado = (int)x.estado,
                politicaCancelacionDias = x.politicaCancelacionDias,
                updateAt = x.updateAt
            })
            .ToList()
            .AsReadOnly();

        return Result.Success<IReadOnlyList<ContratoDto>>(dto);
    }
}