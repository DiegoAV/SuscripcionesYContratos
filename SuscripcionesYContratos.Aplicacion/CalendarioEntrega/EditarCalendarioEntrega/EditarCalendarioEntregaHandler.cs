using Joseco.DDD.Core.Abstractions;
using Joseco.DDD.Core.Results;
using MediatR;
using SuscripcionesYContratos.Dominio.Entregas;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Aplicacion.CalendarioEntrega.EditarCalendarioEntrega
{
    public sealed class EditarCalendarioEntregaHandler : IRequestHandler<EditarCalendarioEntregaCommand, Result<Guid>>
    {
        private readonly ICalendarioEntregaRepo _repo;
        private readonly IUnitOfWork _unitOfWork;

        public EditarCalendarioEntregaHandler(ICalendarioEntregaRepo repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(EditarCalendarioEntregaCommand request, CancellationToken cancellationToken)
        {
            var entrega = await _repo.GetByIdAsync(request.entregaId);
            if (entrega is null)
                return Result.Failure<Guid>(CalendarioEntregaError.CalendarioEntregaNoEncontrado);

            // Solo una de las “acciones” debería venir activa
            var acciones = (request.nuevaHora.HasValue ? 1 : 0) + (request.reprogramarFecha ? 1 : 0) + (request.cancelar ? 1 : 0);
            if (acciones != 1)
                return Result.Failure<Guid>(CalendarioEntregaError.CalendarioEntregaInvalido);

            var hoy = request.hoyOverride ?? DateOnly.FromDateTime(DateTime.Today);

            // 1) Editar SOLO hora: permitido sin regla de 2 días, no mueve la fecha
            if (request.nuevaHora.HasValue)
            {
                entrega.ReprogramarEntrega(entrega.fecha, request.nuevaHora.Value);
                await _repo.UpdateAsync(entrega);
                await _unitOfWork.CommitAsync(cancellationToken);
                return Result.Success(entrega.Id);
            }

            // A partir de aquí aplica regla: “si quiere cancelar o modificar fecha debe faltar dos dias”
            var diasHastaEntrega = entrega.fecha.DayNumber - hoy.DayNumber;
            if (diasHastaEntrega < 2)
                return Result.Failure<Guid>(CalendarioEntregaError.CalendarioEntregaInvalido);

            // 2) Reprogramar FECHA: mover a “última posición”, es decir, última fecha + 1 día
            if (request.reprogramarFecha)
            {
                var ultima = await _repo.GetUltimaEntregaDeContratoAsync(entrega.contratoId, cancellationToken);
                if (ultima is null)
                    return Result.Failure<Guid>(CalendarioEntregaError.ContratoNoExistente);

                var nuevaFecha = ultima.fecha.AddDays(1);

                // Mantén la hora original al mover de posición
                entrega.ReprogramarEntrega(nuevaFecha, entrega.hora);

                await _repo.UpdateAsync(entrega);
                await _unitOfWork.CommitAsync(cancellationToken);
                return Result.Success(entrega.Id);
            }

            // 3) Cancelar: tu dominio aún no tiene método Cancelar(), así que necesito que confirmes
            // si quieres:
            //   A) implementar CalendarioEntrega.Cancelar() (recomendado), o
            //   B) tratar “cancelar” como “mover al final” (no es realmente cancelar).
            return Result.Failure<Guid>(CalendarioEntregaError.CalendarioEntregaInvalido);
        }
    }
}