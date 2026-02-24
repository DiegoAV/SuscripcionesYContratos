using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Core.Abstracciones
{
    public abstract class AggregateRoot : Entity
    {
        protected AggregateRoot(Guid id) : base(id) { }
        protected AggregateRoot() { }
    }
}
