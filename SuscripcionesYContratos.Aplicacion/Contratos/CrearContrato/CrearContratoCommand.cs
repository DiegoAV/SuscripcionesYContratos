using Joseco.DDD.Core.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Aplicacion.Contratos.CrearContrato
{
    public record CrearContratoCommand(Guid pacienteId, Guid suscripcionId, Guid planId, TimeOnly hora, DateOnly inicio, DateOnly fin, int politicaCancelacionDias) :IRequest<Result<Guid>>;

}
