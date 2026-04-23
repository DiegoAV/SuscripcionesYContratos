using Joseco.DDD.Core.Results;
using MediatR;
using System;

namespace SuscripcionesYContratos.Aplicacion.CalendarioEntrega.EditarCalendarioEntrega
{
    public sealed record EditarCalendarioEntregaCommand(
        Guid entregaId,
        TimeOnly? nuevaHora,
        bool reprogramarFecha,          // true => aplica regla “faltan 2 días y pasa al final”
        bool cancelar,                  // true => (mismo criterio de 2 días, ver handler)
        DateOnly? hoyOverride = null    // útil para tests; en prod será null
    ) : IRequest<Result<Guid>>;
}