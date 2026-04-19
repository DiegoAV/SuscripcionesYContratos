using System;
using SuscripcionesYContratos.Aplicacion.Contratos.DTO;
using Xunit;

namespace SuscripcionesYContratos.UnitTests.Aplicacion.Contratos;

public sealed class ContratoDtoTests
{
    [Fact]
    public void Init_AsignaYLeePropiedades()
    {
        var id = Guid.NewGuid();
        var pacienteId = Guid.NewGuid();
        var suscripcionId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var hora = new TimeOnly(6, 30, 0);
        var inicio = new DateOnly(2026, 04, 10);
        var fin = new DateOnly(2026, 04, 17);
        var updateAt = new DateTime(2026, 04, 19, 12, 0, 0, DateTimeKind.Utc);

        var dto = new ContratoDto
        {
            Id = id,
            pacienteId = pacienteId,
            suscripcionId = suscripcionId,
            planId = planId,
            hora = hora,
            inicio = inicio,
            fin = fin,
            incluyeFinDeSemana = false,
            cantidadEntregas = 5,
            precioTotal = 99.95m,
            estado = 1,
            politicaCancelacionDias = 2,
            updateAt = updateAt
        };

        Assert.Equal(id, dto.Id);
        Assert.Equal(pacienteId, dto.pacienteId);
        Assert.Equal(suscripcionId, dto.suscripcionId);
        Assert.Equal(planId, dto.planId);
        Assert.Equal(hora, dto.hora);
        Assert.Equal(inicio, dto.inicio);
        Assert.Equal(fin, dto.fin);
        Assert.False(dto.incluyeFinDeSemana);
        Assert.Equal(5, dto.cantidadEntregas);
        Assert.Equal(99.95m, dto.precioTotal);
        Assert.Equal(1, dto.estado);
        Assert.Equal(2, dto.politicaCancelacionDias);
        Assert.Equal(updateAt, dto.updateAt);
    }

    [Fact]
    public void Record_ConMismosValores_EsEqual_YMismoHashCode()
    {
        var id = Guid.NewGuid();
        var pacienteId = Guid.NewGuid();
        var suscripcionId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var hora = new TimeOnly(7, 0, 0);
        var inicio = new DateOnly(2026, 04, 10);
        var fin = new DateOnly(2026, 04, 15);
        var updateAt = new DateTime(2026, 04, 19, 12, 0, 0, DateTimeKind.Utc);

        var a = new ContratoDto
        {
            Id = id,
            pacienteId = pacienteId,
            suscripcionId = suscripcionId,
            planId = planId,
            hora = hora,
            inicio = inicio,
            fin = fin,
            incluyeFinDeSemana = true,
            cantidadEntregas = 3,
            precioTotal = 30m,
            estado = 2,
            politicaCancelacionDias = 1,
            updateAt = updateAt
        };

        var b = new ContratoDto
        {
            Id = id,
            pacienteId = pacienteId,
            suscripcionId = suscripcionId,
            planId = planId,
            hora = hora,
            inicio = inicio,
            fin = fin,
            incluyeFinDeSemana = true,
            cantidadEntregas = 3,
            precioTotal = 30m,
            estado = 2,
            politicaCancelacionDias = 1,
            updateAt = updateAt
        };

        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ToString_ContieneNombreDelTipo_YAlgunosCampos()
    {
        var dto = new ContratoDto
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            pacienteId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            suscripcionId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            planId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            hora = new TimeOnly(6, 30, 0),
            inicio = new DateOnly(2026, 04, 10),
            fin = new DateOnly(2026, 04, 17),
            incluyeFinDeSemana = false,
            cantidadEntregas = 5,
            precioTotal = 100m,
            estado = 1,
            politicaCancelacionDias = 2,
            updateAt = null
        };

        var s = dto.ToString();

        Assert.Contains(nameof(ContratoDto), s);
        Assert.Contains(dto.Id.ToString(), s);
        Assert.Contains(dto.planId.ToString(), s);
    }
}
