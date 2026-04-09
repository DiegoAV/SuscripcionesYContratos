using Joseco.DDD.Core.Results;
using SuscripcionesYContratos.Dominio.Contrato;
using Xunit;

namespace SuscripcionesYContratos.UnitTests.Dominio.Contrato;

public sealed class ContratosTests
{
    private static Contratos NuevoContratoBase()
    {
        return new Contratos(
            id: Guid.NewGuid(),
            pacienteId: Guid.NewGuid(),
            suscripcionId: Guid.NewGuid(),
            planId: Guid.NewGuid(),
            hora: new TimeOnly(7, 0),
            inicio: new DateOnly(2026, 04, 06),
            fin: new DateOnly(2026, 04, 06),
            incluyeFinDeSemana: true,
            cantidadEntregas: 1,
            precioTotal: 10m,
            politicaCancelacionDias: 2);
    }

    [Theory]
    [InlineData(6, 29)]
    [InlineData(9, 01)]
    public void SetHora_FueraDeRango_LanzaDomainException(int h, int m)
    {
        var c = NuevoContratoBase();
        Assert.Throws<DomainException>(() => c.SetHora(new TimeOnly(h, m)));
    }

    [Theory]
    [InlineData(6, 30)]
    [InlineData(9, 00)]
    public void SetHora_EnRango_NoLanza(int h, int m)
    {
        var c = NuevoContratoBase();
        c.SetHora(new TimeOnly(h, m));
        Assert.Equal(new TimeOnly(h, m), c.hora);
        Assert.NotNull(c.updateAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SetPoliticaCancelacionDias_Invalida_LanzaDomainException(int dias)
    {
        var c = NuevoContratoBase();
        Assert.Throws<DomainException>(() => c.SetPoliticaCancelacionDias(dias));
    }

    [Fact]
    public void CancelarContrato_DosVeces_LanzaDomainException()
    {
        var c = NuevoContratoBase();
        c.cancelarContrato();
        Assert.Throws<DomainException>(() => c.cancelarContrato());
    }
}