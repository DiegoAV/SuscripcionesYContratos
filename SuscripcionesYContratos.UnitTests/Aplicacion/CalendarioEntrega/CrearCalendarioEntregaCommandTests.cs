using Joseco.DDD.Core.Results;
using SuscripcionesYContratos.Aplicacion.CalendarioEntrega.CrearCalendarioEntrega;
using System;
using Xunit;

namespace SuscripcionesYContratos.UnitTests.Aplicacion.CalendarioEntrega;

public sealed class CrearCalendarioEntregaCommandTests
{
    [Fact]
    public void Propiedades_SeAsignanCorrectamente()
    {
        var contratoId = Guid.NewGuid();
        var fecha = new DateOnly(2026, 04, 06);
        var hora = new TimeOnly(7, 0);

        var cmd = new CrearCalendarioEntregaCommand(
            contratoId: contratoId,
            fechaEntrega: fecha,
            horaEntrega: hora);

        Assert.Equal(contratoId, cmd.contratoId);
        Assert.Equal(fecha, cmd.fechaEntrega);
        Assert.Equal(hora, cmd.horaEntrega);
    }

    [Fact]
    public void Record_ConMismosValores_EsEqual_YMismoHashCode()
    {
        var contratoId = Guid.NewGuid();
        var fecha = new DateOnly(2026, 04, 06);
        var hora = new TimeOnly(7, 0);

        var a = new CrearCalendarioEntregaCommand(contratoId, fecha, hora);
        var b = new CrearCalendarioEntregaCommand(contratoId, fecha, hora);

        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void WithExpression_CreaNuevaInstancia_ConSoloUnCampoCambiado()
    {
        var contratoId = Guid.NewGuid();
        var fecha = new DateOnly(2026, 04, 06);
        var hora = new TimeOnly(7, 0);

        var original = new CrearCalendarioEntregaCommand(contratoId, fecha, hora);

        var nuevaFecha = new DateOnly(2026, 04, 07);
        var modificado = original with { fechaEntrega = nuevaFecha };

        Assert.Equal(original.contratoId, modificado.contratoId);
        Assert.Equal(original.horaEntrega, modificado.horaEntrega);
        Assert.Equal(nuevaFecha, modificado.fechaEntrega);

        Assert.NotEqual(original, modificado);
    }

    [Fact]
    public void ImplementaIRequest_DeResultGuid()
    {
        // Esto fuerza a ejecutar parte del metadata de tipos en runtime y cubre el contrato del command.
        Assert.True(typeof(MediatR.IRequest<Result<Guid>>).IsAssignableFrom(typeof(CrearCalendarioEntregaCommand)));
    }

    [Fact]
    public void ToString_ContieneNombreDelTipo_YValores()
    {
        var cmd = new CrearCalendarioEntregaCommand(
            contratoId: Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            fechaEntrega: new DateOnly(2026, 04, 06),
            horaEntrega: new TimeOnly(7, 0));

        var s = cmd.ToString();

        Assert.Contains(nameof(CrearCalendarioEntregaCommand), s);
        Assert.Contains(cmd.contratoId.ToString(), s);
        Assert.Contains(cmd.fechaEntrega.ToString(), s);
        Assert.Contains(cmd.horaEntrega.ToString(), s);
    }
}
