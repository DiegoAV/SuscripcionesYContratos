using System;
using SuscripcionesYContratos.Aplicacion.Suscripciones.DTO;
using Xunit;

namespace SuscripcionesYContratos.UnitTests.Aplicacion.Suscripciones;

public sealed class SuscripcionDtoTests
{
    [Fact]
    public void SettersGetters_AsignaYLeePropiedades()
    {
        var id = Guid.NewGuid();
        var updateAt = new DateTime(2026, 04, 19, 12, 0, 0, DateTimeKind.Utc);

        var dto = new SuscripcionDto
        {
            Id = id,
            nombre = "Plan A",
            descripcion = "Desc",
            cantidadDias = 5,
            precioDia = 10.5m,
            updateAt = updateAt
        };

        Assert.Equal(id, dto.Id);
        Assert.Equal("Plan A", dto.nombre);
        Assert.Equal("Desc", dto.descripcion);
        Assert.Equal(5, dto.cantidadDias);
        Assert.Equal(10.5m, dto.precioDia);
        Assert.Equal(updateAt, dto.updateAt);
    }

    [Fact]
    public void Record_ConMismasPropiedades_EsEqual_YMismoHashCode()
    {
        var id = Guid.NewGuid();
        var updateAt = new DateTime(2026, 04, 19, 12, 0, 0, DateTimeKind.Utc);

        var a = new SuscripcionDto
        {
            Id = id,
            nombre = "Plan A",
            descripcion = "Desc",
            cantidadDias = 5,
            precioDia = 10m,
            updateAt = updateAt
        };

        var b = new SuscripcionDto
        {
            Id = id,
            nombre = "Plan A",
            descripcion = "Desc",
            cantidadDias = 5,
            precioDia = 10m,
            updateAt = updateAt
        };

        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ToString_DevuelveRepresentacionConNombreDelTipo()
    {
        var dto = new SuscripcionDto
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            nombre = "Plan A",
            descripcion = "Desc",
            cantidadDias = 5,
            precioDia = 10m,
            updateAt = null
        };

        var s = dto.ToString();

        Assert.Contains(nameof(SuscripcionDto), s);
        Assert.Contains(dto.Id.ToString(), s);
    }
}
