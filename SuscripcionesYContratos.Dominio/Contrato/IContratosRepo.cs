using Joseco.DDD.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Dominio.Contrato
{
    public interface IContratosRepo : IRepository<Contratos>
    {
        Task UpdateAsync(Contratos contrato);
    }
}
