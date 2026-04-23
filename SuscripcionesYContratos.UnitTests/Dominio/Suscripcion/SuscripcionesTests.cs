using System;
using Joseco.DDD.Core.Results;
using SuscripcionesYContratos.Dominio.Suscripcion;
using Xunit;

namespace SuscripcionesYContratos.UnitTests.Dominio.Suscripcion;

public sealed class SuscripcionesTests
{
    private static Suscripciones NuevaSuscripcionBase()
    {
        return new Suscripciones(
            suscripcionID: Guid.NewGuid(),
            nombre: "Plan Base",
            descripcion: "Descripcion base",
            cantidadDias: 5,
            precioDia: 10m);
    }

    [Fact]
    public void Constructor_AsignaPropiedades_Y_NoSeteaUpdateAt()
    {
        var id = Guid.NewGuid();

        var s = new Suscripciones(
            suscripcionID: id,
            nombre: "Plan A",
            descripcion: "Desc A",
            cantidadDias: 3,
            precioDia: 2.5m);

        Assert.Equal(id, s.Id);
        Assert.Equal("Plan A", s.nombre);
        Assert.Equal("Desc A", s.descripcion);
        Assert.Equal(3, s.cantidadDias);
        Assert.Equal(2.5m, s.precioDia);
        Assert.Null(s.updateAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetNombre_Invalido_LanzaDomainException(string? nuevoNombre)
    {
        var s = NuevaSuscripcionBase();
        Assert.Throws<DomainException>(() => s.SetNombre(nuevoNombre!));
    }

    [Fact]
    public void SetNombre_Valido_ActualizaValor_Trim_Y_updateAt()
    {
        var s = NuevaSuscripcionBase();

        s.SetNombre("  Nuevo  ");

        Assert.Equal("Nuevo", s.nombre);
        Assert.NotNull(s.updateAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetDescripcion_Invalida_LanzaDomainException(string? nuevaDescripcion)
    {
        var s = NuevaSuscripcionBase();
        Assert.Throws<DomainException>(() => s.SetDescripcion(nuevaDescripcion!));
    }

    [Fact]
    public void SetDescripcion_Valida_ActualizaValor_Trim_Y_updateAt()
    {
        var s = NuevaSuscripcionBase();

        s.SetDescripcion("  Desc nueva  ");

        Assert.Equal("Desc nueva", s.descripcion);
        Assert.NotNull(s.updateAt);
    }

    [Fact]
    public void SetPrecioDia_Negativo_LanzaDomainException()
    {
        var s = NuevaSuscripcionBase();
        Assert.Throws<DomainException>(() => s.SetPrecioDia(-0.01m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10.75)]
    public void SetPrecioDia_Valido_ActualizaValor_Y_updateAt(decimal nuevoPrecio)
    {
        var s = NuevaSuscripcionBase();

        s.SetPrecioDia(nuevoPrecio);

        Assert.Equal(nuevoPrecio, s.precioDia);
        Assert.NotNull(s.updateAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SetCantidadDias_Invalida_LanzaDomainException(int dias)
    {
        var s = NuevaSuscripcionBase();
        Assert.Throws<DomainException>(() => s.SetCantidadDias(dias));
    }

    [Fact]
    public void SetCantidadDias_Valida_ActualizaValor_Y_updateAt()
    {
        var s = NuevaSuscripcionBase();

        s.SetCantidadDias(7);

        Assert.Equal(7, s.cantidadDias);
        Assert.NotNull(s.updateAt);
    }
}
