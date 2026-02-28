using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Infraestructura.Persistencia
{
    public interface IDatabase : IDisposable
    {
        void Migrate();
    }
}
