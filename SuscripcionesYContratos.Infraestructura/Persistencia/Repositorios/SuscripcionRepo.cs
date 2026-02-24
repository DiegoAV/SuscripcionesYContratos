using SuscripcionesYContratos.Dominio.Suscripcion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Infraestructura.Persistencia.Repositorios
{
    internal class SuscripcionRepo : ISuscripcionesRepo
    {
        public Task AddAsync(Suscripciones entity)
        {
            return Task.CompletedTask;
        }

        public Task<Suscripciones?> GetByIdAsync(Guid id, bool readOnly = false)
        {
            return Task.FromResult<Suscripciones?>(null);
        }

        public Task UpdateAsync(Suscripciones suscripcion)
        {
            return Task.CompletedTask;
        }
    }
}
