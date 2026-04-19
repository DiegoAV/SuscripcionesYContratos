using System;
using SuscripcionesYContratos.Dominio.Entregas.Eventos;
using Xunit;

namespace SuscripcionesYContratos.UnitTests.Dominio.Entregas;

public sealed class CalendarioEntregaDomainEventTests
{
    [Fact]
    public void Constructor_AsignaPropiedades()
    {
        var entregaId = Guid.NewGuid();
        var contratoId = Guid.NewGuid();
        var fecha = new DateOnly(2026, 04, 06);
        var hora = new TimeOnly(9, 0);
        const int estado = 0;
        var occurred = new DateTime(2026, 04, 01, 10, 30, 0, DateTimeKind.Utc);

        var ev = new CalendarioEntregaDomainEvent(entregaId, contratoId, fecha, hora, estado, occurred);

        Assert.Equal(entregaId, ev.entregaId);
        Assert.Equal(contratoId, ev.contratoId);
        Assert.Equal(fecha, ev.fecha);
        Assert.Equal(hora, ev.hora);
        Assert.Equal(estado, ev.estado);
        Assert.Equal(occurred, ev.occurredOnUtc);
    }

    [Fact]
    public void MismoContenido_DistintasInstancias_TienenMismasPropiedadesDeDominio()
    {
        var entregaId = Guid.NewGuid();
        var contratoId = Guid.NewGuid();
        var fecha = new DateOnly(2026, 04, 06);
        var hora = new TimeOnly(9, 0);
        const int estado = 1;
        var occurred = new DateTime(2026, 04, 01, 10, 30, 0, DateTimeKind.Utc);

        var a = new CalendarioEntregaDomainEvent(entregaId, contratoId, fecha, hora, estado, occurred);
        var b = new CalendarioEntregaDomainEvent(entregaId, contratoId, fecha, hora, estado, occurred);

        // Importante: no comparar a y b con Assert.Equal() porque DomainEvent mete Id/OccuredOn distintos
        Assert.NotEqual(a.Id, b.Id); // si DomainEvent expone Id
        Assert.Equal(a.entregaId, b.entregaId);
        Assert.Equal(a.contratoId, b.contratoId);
        Assert.Equal(a.fecha, b.fecha);
        Assert.Equal(a.hora, b.hora);
        Assert.Equal(a.estado, b.estado);
        Assert.Equal(a.occurredOnUtc, b.occurredOnUtc);
    }

    [Fact]
    public void With_CreaNuevaInstancia_YNoModificaOriginal()
    {
        var entregaId = Guid.NewGuid();
        var contratoId = Guid.NewGuid();

        var original = new CalendarioEntregaDomainEvent(
            entregaId,
            contratoId,
            new DateOnly(2026, 04, 06),
            new TimeOnly(9, 0),
            estado: 0,
            occurredOnUtc: new DateTime(2026, 04, 01, 10, 30, 0, DateTimeKind.Utc));

        var cambiado = original with { estado = 1 };

        Assert.Equal(0, original.estado);
        Assert.Equal(1, cambiado.estado);
        Assert.NotSame(original, cambiado);
        Assert.Equal(original.entregaId, cambiado.entregaId);
        Assert.Equal(original.contratoId, cambiado.contratoId);
    }

    [Fact]
    public void ToString_ContieneNombreYValoresClave()
    {
        var entregaId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var contratoId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var ev = new CalendarioEntregaDomainEvent(
            entregaId,
            contratoId,
            new DateOnly(2026, 04, 06),
            new TimeOnly(9, 0),
            estado: 0,
            occurredOnUtc: new DateTime(2026, 04, 01, 10, 30, 0, DateTimeKind.Utc));

        var s = ev.ToString();

        Assert.Contains(nameof(CalendarioEntregaDomainEvent), s);
        Assert.Contains(entregaId.ToString(), s);
        Assert.Contains(contratoId.ToString(), s);
    }
}
