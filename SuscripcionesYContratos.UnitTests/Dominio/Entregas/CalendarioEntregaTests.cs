using Joseco.DDD.Core.Results;
using SuscripcionesYContratos.Dominio.Entregas;
using Xunit;

namespace SuscripcionesYContratos.UnitTests.Dominio.Entregas;

public sealed class CalendarioEntregaTests
{
    [Fact]
    public void Constructor_CreaProgramado_YPermiteReprogramar()
    {
        var e = new CalendarioEntrega(Guid.NewGuid(), new DateOnly(2026, 04, 06), new TimeOnly(9, 0));
        e.ReprogramarEntrega(new DateOnly(2026, 04, 07), new TimeOnly(8, 30));

        Assert.Equal(CalendarioEntregaEstado.Reprogramado, e.estado);
        Assert.Equal(new DateOnly(2026, 04, 07), e.fecha);
        Assert.Equal(new TimeOnly(8, 30), e.hora);
        Assert.NotNull(e.updateAt);
    }

    [Fact]
    public void ReprogramarEntrega_SiYaReprogramado_LanzaDomainException()
    {
        var e = new CalendarioEntrega(Guid.NewGuid(), new DateOnly(2026, 04, 06), new TimeOnly(9, 0));
        e.ReprogramarEntrega(new DateOnly(2026, 04, 07), new TimeOnly(8, 30));

        Assert.Throws<DomainException>(() => e.ReprogramarEntrega(new DateOnly(2026, 04, 08), new TimeOnly(8, 0)));
    }
}