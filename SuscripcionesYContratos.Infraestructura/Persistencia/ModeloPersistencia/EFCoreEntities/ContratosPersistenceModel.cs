using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuscripcionesYContratos.Infraestructura.Persistencia.ModeloPersistencia.EFCoreEntities
{
    [Table("Contratos")]
    internal class ContratosPersistenceModel
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("pacienteId")]
        public Guid pacienteId { get; set; }

        [Required]
        [Column("suscripcionId")]
        public Guid suscripcionId { get; set; }

        public required SuscripcionesPersistenceModel Suscripcion { get; set; }

        [Required]
        [Column("planId")]
        public Guid planId { get; set; }

        [Required]
        [Column("hora")]
        public TimeOnly hora { get; set; }

        [Required]
        [Column("inicio")]
        public DateOnly inicio { get; set; }

        [Required]
        [Column("fin")]
        public DateOnly fin { get; set; }

        [Required]
        [Column("incluyeFinDeSemana")]
        public bool incluyeFinDeSemana { get; set; }

        [Required]
        [Column("cantidadEntregas")]
        public int cantidadEntregas { get; set; }

        [Required]
        [Column("precioTotal")]
        public decimal precioTotal { get; set; }

        [Required]
        [Column("estado")]
        public int estado { get; set; }

        [Required]
        [Column("politicaCancelacionDias")]
        public int politicaCancelacionDias { get; set; }

        [Column("updateAt")]
        public DateTime? updateAt { get; set; }

        public List<CalendarioEntregaPersistenceModel> CalendarioEntregas { get; set; } = new();
    }
}
