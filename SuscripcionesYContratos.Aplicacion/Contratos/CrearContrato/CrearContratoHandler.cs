using Joseco.DDD.Core.Abstractions;
using Joseco.DDD.Core.Results;
using MediatR;
using SuscripcionesYContratos.Dominio.Contrato;
using SuscripcionesYContratos.Dominio.Entregas;
using SuscripcionesYContratos.Dominio.Suscripcion;

namespace SuscripcionesYContratos.Aplicacion.Contratos.CrearContrato
{
    public sealed class CrearContratoHandler : IRequestHandler<CrearContratoCommand, Result<Guid>>
    {
        private readonly IContratosRepo _contratosRepo;
        private readonly ISuscripcionesRepo _suscripcionesRepo;
        private readonly ICalendarioEntregaRepo _calendarioEntregaRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CrearContratoHandler(
            IContratosRepo contratosRepo,
            ISuscripcionesRepo suscripcionesRepo,
            ICalendarioEntregaRepo calendarioEntregaRepo,
            IUnitOfWork unitOfWork)
        {
            _contratosRepo = contratosRepo;
            _suscripcionesRepo = suscripcionesRepo;
            _calendarioEntregaRepo = calendarioEntregaRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CrearContratoCommand request, CancellationToken cancellationToken)
        {
            var suscripcion = await _suscripcionesRepo.GetByIdAsync(request.suscripcionId, readOnly: true);

            if (suscripcion is null)
                return Result.Failure<Guid>(SuscripcionError.SuscripcionNoEncontrada);

            if (suscripcion.cantidadDias <= 0)
                return Result.Failure<Guid>(SuscripcionError.CantidadDiasInvalida);

            // cantidadDias = días calendario (fin inclusive)
            var fin = request.inicio.AddDays(suscripcion.cantidadDias - 1);

            var cantidadEntregas = CalcularCantidadEntregas(request.inicio, fin, request.incluyeFinDeSemana);
            var precioTotal = suscripcion.precioDia * cantidadEntregas;

            var contrato = new Dominio.Contrato.Contratos(
                id: Guid.NewGuid(),
                pacienteId: request.pacienteId,
                suscripcionId: request.suscripcionId,
                planId: request.planId,
                hora: request.hora,
                inicio: request.inicio,
                fin: fin,
                incluyeFinDeSemana: request.incluyeFinDeSemana,
                cantidadEntregas: cantidadEntregas,
                precioTotal: precioTotal,
                politicaCancelacionDias: request.politicaCancelacionDias);

            contrato.SetHora(request.hora);
            contrato.SetPoliticaCancelacionDias(request.politicaCancelacionDias);

            await _contratosRepo.AddAsync(contrato);
            // agrega 1 contrato, pero no se han agregado las entregas al calendario, por eso se hace commit aquí para generar el Id del contrato y usarlo en las entregas
            await _unitOfWork.CommitAsync(cancellationToken);


            // Crear CalendarioEntrega por cada día del rango (inclusive)
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

            await _unitOfWork.CommitAsync(cancellationToken);

            //await _calendarioEntregaRepo.

            return Result.Success(contrato.Id);
        }

        private static int CalcularCantidadEntregas(DateOnly inicio, DateOnly fin, bool incluyeFinDeSemana)
        {
            if (fin < inicio)
                return 0;

            if (incluyeFinDeSemana)
                return fin.DayNumber - inicio.DayNumber + 1;

            var total = 0;
            var fecha = inicio;

            while (fecha <= fin)
            {
                if (EsDiaLaborable(fecha))
                    total++;

                fecha = fecha.AddDays(1);
            }

            return total;
        }

        private static bool EsDiaLaborable(DateOnly date)
        {
            var dt = date.ToDateTime(TimeOnly.MinValue);
            return dt.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday;
        }
    }
}
