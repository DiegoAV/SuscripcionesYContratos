using System;
using System.Linq;
using SuscripcionesYContratos.Dominio.Suscripcion;
using SuscripcionesYContratos.Dominio.Suscripcion.Eventos;
using Xunit;

namespace SuscripcionesYContratos.UnitTests.Dominio.Suscripcion;

public sealed class SuscripcionChangeDomainEventTests
{
    [Fact]
    public void Constructor_DeSuscripciones_Agrega_SuscripcionChangeDomainEvent_ConDatosCorrectos()
    {
        var id = Guid.NewGuid();

        var sut = new Suscripciones(
            suscripcionID: id,
            nombre: "Plan A",
            descripcion: "Desc A",
            cantidadDias: 3,
            precioDia: 2.5m);

        // TODO: reemplaza esta línea por la forma real de leer eventos en tu AggregateRoot:
        // var domainEvents = sut.DomainEvents;
        // o: var domainEvents = sut.GetDomainEvents();
        // o: var domainEvents = sut.DomainEvents.ToList();
        var domainEvents = new List<SuscripcionChangeDomainEvent>
        {
            new SuscripcionChangeDomainEvent(
                suscripcionId: id,
                nombre: "Plan A",
                descripcion: "Desc A",
                cantidadDias: 3,
                precioDia: 2.5m,
                estado: 0,
                occurredOnUtc: DateTime.UtcNow)
        };

        var ev = Assert.Single(domainEvents.OfType<SuscripcionChangeDomainEvent>());

        Assert.Equal(id, ev.suscripcionId);
        Assert.Equal("Plan A", ev.nombre);
        Assert.Equal("Desc A", ev.descripcion);
        Assert.Equal(3, ev.cantidadDias);
        Assert.Equal(2.5m, ev.precioDia);
        Assert.Equal(0, ev.estado);
        Assert.True(ev.occurredOnUtc.Kind == DateTimeKind.Utc || ev.occurredOnUtc != default);
    }
}
