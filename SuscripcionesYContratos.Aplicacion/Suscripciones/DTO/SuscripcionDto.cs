using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Aplicacion.Suscripciones.DTO
{
    public record SuscripcionDto
    {
        public Guid Id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int cantidadDias { get; set; }
        public decimal precioDia { get; set; }
        public DateTime? updateAt { get; set; }

    }
}
