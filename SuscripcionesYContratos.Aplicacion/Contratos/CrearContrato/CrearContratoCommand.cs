using Joseco.DDD.Core.Results;
using MediatR;

namespace SuscripcionesYContratos.Aplicacion.Contratos.CrearContrato
{
    public sealed record CrearContratoCommand(
        Guid pacienteId,
        Guid suscripcionId,
        Guid planId,
        TimeOnly hora,
        DateOnly inicio,
        bool incluyeFinDeSemana,
        int politicaCancelacionDias
    ) : IRequest<Result<Guid>>;
}
