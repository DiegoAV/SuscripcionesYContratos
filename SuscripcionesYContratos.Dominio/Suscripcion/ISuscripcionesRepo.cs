using Joseco.DDD.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Dominio.Suscripcion
{
    public interface ISuscripcionesRepo : IRepository<Suscripciones>
    {
            Task UpdateAsync(Suscripciones suscripcion);
    }
}
