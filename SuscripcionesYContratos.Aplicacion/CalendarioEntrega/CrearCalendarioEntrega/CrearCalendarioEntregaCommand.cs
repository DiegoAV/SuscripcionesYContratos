using Joseco.DDD.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Aplicacion.CalendarioEntrega.CrearCalendarioEntrega
{
    public sealed record CrearCalendarioEntregaCommand(
        Guid contratoId,
        DateOnly fechaEntrega,
        TimeOnly horaEntrega
    ) : MediatR.IRequest<Result<Guid>>;
}
