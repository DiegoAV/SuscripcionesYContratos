using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuscripcionesYContratos.Infraestructura.Persistencia.ModeloPersistencia.EFCoreEntities
{
    [Table("Suscripciones")]
    internal class SuscripcionesPersistenceModel
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("nombre")]
        public required string nombre { get; set; }

        [Required]
        [Column("descripcion")]
        public required string descripcion { get; set; }

        [Required]
        [Column("cantidadDias")]
        public int cantidadDias { get; set; }

        [Required]
        [Column("precioDia")]
        public decimal precioDia { get; set; }

        [Column("updateAt")]
        public DateTime? updateAt { get; set; }
    }
}
