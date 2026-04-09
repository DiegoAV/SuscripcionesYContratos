using Joseco.DDD.Core.Abstractions;
using Joseco.DDD.Core.Results;
using MediatR;
using SuscripcionesYContratos.Dominio.Contrato;
using SuscripcionesYContratos.Dominio.Entregas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SuscripcionesYContratos.Aplicacion.CalendarioEntrega.CrearCalendarioEntrega
{
    public sealed class CrearCalendarioEntregaHandler : IRequestHandler<CrearCalendarioEntregaCommand, Result<Guid>>
    {
        private readonly ICalendarioEntregaRepo _calendarioEntregaRepo;
        private readonly IContratosRepo _contratosRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CrearCalendarioEntregaHandler(
            ICalendarioEntregaRepo calendarioEntregaRepo,
            IContratosRepo contratosRepo,
            IUnitOfWork unitOfWork)
        {
            _calendarioEntregaRepo = calendarioEntregaRepo;
            _contratosRepo = contratosRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(CrearCalendarioEntregaCommand request, CancellationToken cancellationToken)
        {
            var contrato = await _contratosRepo.GetByIdAsync(request.contratoId, readOnly: true);
            if (contrato is null)
            {
                return Result.Failure<Guid>(CalendarioEntregaError.ContratoNoExistente);
            }

            var fecha = contrato.inicio;
            while (fecha <= contrato.fin)
            {
                if (contrato.incluyeFinDeSemana || EsDiaLaborable(fecha))
                {
                    var entrega = new Dominio.Entregas.CalendarioEntrega(
                        contratoId: contrato.Id,
                        fecha: fecha,
                        hora: contrato.hora);

                    await _calendarioEntregaRepo.AddAsync(entrega);
                }

                fecha = fecha.AddDays(1);
            }

            return Result.Success(contrato.Id);

        }

        private bool EsDiaLaborable(DateOnly date)
        {
            var dt = date.ToDateTime(TimeOnly.MinValue);
            return dt.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday;
        }
    }
}
