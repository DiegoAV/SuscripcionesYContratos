using Joseco.DDD.Core.Abstractions;
using Joseco.DDD.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Dominio.Suscripcion
{
    public class Suscripciones : AggregateRoot
    {
        public String nombre { get; private set; } // Asesoramiento nutricional, Plan de alimentación Quincenal, Plan de alimentación Mensual, etc.
        public String descripcion { get; private set; } // Descripción detallada de la suscripción
        public int cantidadEntregas { get; private set; } // Cantidad de entregas que incluye la suscripción, por ejemplo, 10 entregas para el plan quincenal sin Fin de Semana o 14 entregas si incluye fin de semana
        public decimal precio { get; private set; }
        public bool incluyeFinDeSemana { get; private set; } // TRUE = Indica si la suscripción incluye entregas los fines de semana
        public DateTime? updateAt { get; private set; }

        private Suscripciones()
        {
        }
        public Suscripciones(Guid suscripcionID, string nombre, string descripcion, int cantidadEntregas, decimal precio, bool incluyeFinDeSemana) : base(suscripcionID)
        {
            this.nombre = nombre;   
            this.descripcion = descripcion;
            this.cantidadEntregas = cantidadEntregas;
            this.precio = precio;
            this.incluyeFinDeSemana = incluyeFinDeSemana;

        }
         public void SetPrecio(decimal nuevoPrecio)
        {
            if (nuevoPrecio < 0)
                throw new DomainException(SuscripcionError.PrecioInvalido);
            
            this.precio = nuevoPrecio;
            this.updateAt = DateTime.UtcNow;
        }
    }
}
