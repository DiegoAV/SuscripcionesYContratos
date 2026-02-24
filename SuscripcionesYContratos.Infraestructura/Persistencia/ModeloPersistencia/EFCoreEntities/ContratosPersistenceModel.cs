using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Infraestructura.Persistencia.ModeloPersistencia.EFCoreEntities
{
    [Table("Contratos")]
    internal class ContratosPersistenceModel
    {
        [Key]
        [Column("contratoId")]
        public Guid contratoId { get; set; }
        
        [Required]
        [Column("pacienteId")]
        public required Guid pacienteId { get; set; }

        [Required]
        [Column("suscripcionId")]
        public required Guid suscripcionId { get; set; }
        public required SuscripcionesPersistenceModel Suscripcion { get; set; }

        [Required]
        [Column("planId")]
        public required Guid planId { get; set; }

        [Required]
        [Column("hora")]
        public required TimeOnly hora { get; set; }

        [Required]
        [Column("inicio")]
        public required DateOnly inicio { get; set; }

        [Required]
        [Column("fin")]
        public required DateOnly fin { get; set; }

        [Required]
        [Column("estado")]
        public required string estado { get; set; }

        [Required]
        [Column("politicaCancelacionDias")]
        public required int politicaCancelacionDias { get; set; }

        [Column("updateAt")]
        public DateTime? updateAt { get; set; }

        public List<CalendarioEntregaPersistenceModel> CalendarioEntregas { get; set; }

    }
}
