using Joseco.DDD.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Dominio.Entregas
{
    public interface ICalendarioEntregaRepo : IRepository<CalendarioEntrega>
    {
        Task UpdateAsync(CalendarioEntrega calendarioEntrega);
    }
}
