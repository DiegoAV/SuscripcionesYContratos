using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuscripcionesYContratos.Infraestructura.Persistencia.ModeloPersistencia.EFCoreEntities;

[Table("CalendarioEntrega")]
public class CalendarioEntregaPersistenceModel
{
    [Key]
    [Column("Id")]
    public Guid Id { get; set; }

    [Required]
    [Column("contratoId")]
    public required Guid contratoId { get; set; }

    [Required] // <-- necesario para que ValidateModel() produzca ValidationResult en runtime
    public required ContratosPersistenceModel Contrato { get; set; }

    [Required]
    [Column("fecha")]
    public required DateOnly fecha { get; set; }

    [Required]
    [Column("hora")]
    public required TimeOnly hora { get; set; }

    [Required]
    [Column("estado")]
    public required int estado { get; set; }

    [Column("updateAt")]
    public DateTime? updateAt { get; set; }
}
