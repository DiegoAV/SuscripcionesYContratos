using Joseco.DDD.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Dominio.Suscripcion.Eventos
{
    public sealed record SuscripcionChangeDomainEvent(
        Guid suscripcionId,
        string nombre,
        string descripcion,
        int cantidadDias,
        decimal precioDia,
        int estado,
        DateTime occurredOnUtc) : DomainEvent;
}
