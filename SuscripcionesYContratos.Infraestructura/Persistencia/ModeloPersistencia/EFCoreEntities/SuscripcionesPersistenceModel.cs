using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Infraestructura.Persistencia.ModeloPersistencia.EFCoreEntities
{
    [Table("Suscripciones")]
    internal class SuscripcionesPersistenceModel
    {
        [Key]
        [Column("suscripcionID")]
        public Guid suscripcionID { get; set; }

        [Required]
        [Column("nombre")]
        public required string nombre { get; set; }

        [Required]
        [Column("descripcion")]
        public required string descripcion { get; set; }

        [Required]
        [Column("cantidadEntregas")]
        public required int cantidadEntregas { get; set; }

        [Required]
        [Column("precio")]
        public required decimal precio { get; set; }

        [Required]
        [Column("incluyeFinDeSemana")]
        public required bool incluyeFinDeSemana { get; set; }

        [Column("updateAt")]
        public DateTime? updateAt { get; set; }

    }
}
