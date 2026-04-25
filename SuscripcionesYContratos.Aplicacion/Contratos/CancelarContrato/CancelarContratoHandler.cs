using Joseco.DDD.Core.Abstractions;
using Joseco.DDD.Core.Results;
using MediatR;
using SuscripcionesYContratos.Dominio.Contrato;
using SuscripcionesYContratos.Dominio.Entregas;

namespace SuscripcionesYContratos.Aplicacion.Contratos.CancelarContrato;

public sealed class CancelarContratoHandler : IRequestHandler<CancelarContratoCommand, Result<Guid>>
{
    private readonly IContratosRepo _contratosRepo;
    private readonly ICalendarioEntregaRepo _entregasRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CancelarContratoHandler(
        IContratosRepo contratosRepo,
        ICalendarioEntregaRepo entregasRepo,
        IUnitOfWork unitOfWork)
    {
        _contratosRepo = contratosRepo;
        _entregasRepo = entregasRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CancelarContratoCommand request, CancellationToken cancellationToken)
    {
        var contrato = await _contratosRepo.GetByIdAsync(request.contratoId, readOnly: false);
        if (contrato is null)
            return Result.Failure<Guid>(CalendarioEntregaError.ContratoNoExistente);

        var entregas = await _entregasRepo.ListByContratoIdAsync(request.contratoId, readOnly: false, cancellationToken);

        foreach (var entrega in entregas)
        {
            if (entrega.estado == CalendarioEntregaEstado.Entregado)
                continue;

            if (entrega.estado is CalendarioEntregaEstado.Programado or CalendarioEntregaEstado.Reprogramado)
            {
                entrega.Cancelar();
                await _entregasRepo.UpdateAsync(entrega);
            }
        }

        contrato.cancelarContrato();
        await _contratosRepo.UpdateAsync(contrato);

        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success(contrato.Id);
    }
}