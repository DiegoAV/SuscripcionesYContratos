namespace SuscripcionesYContratos.Aplicacion.Contratos.DTO;

public sealed record ContratoDto
{
    public Guid Id { get; init; }
    public Guid pacienteId { get; init; }
    public Guid suscripcionId { get; init; }
    public Guid planId { get; init; }
    public TimeOnly hora { get; init; }
    public DateOnly inicio { get; init; }
    public DateOnly fin { get; init; }
    public bool incluyeFinDeSemana { get; init; }
    public int cantidadEntregas { get; init; }
    public decimal precioTotal { get; init; }
    public int estado { get; init; }
    public int politicaCancelacionDias { get; init; }
    public DateTime? updateAt { get; init; }
}