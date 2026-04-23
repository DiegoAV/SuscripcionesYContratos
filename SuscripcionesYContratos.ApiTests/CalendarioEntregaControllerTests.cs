using System;
using System.Threading;
using System.Threading.Tasks;
using Joseco.DDD.Core.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Language.Flow;
using SuscripcionesYContratos.API.Controllers;
using SuscripcionesYContratos.Aplicacion.CalendarioEntrega.CrearCalendarioEntrega;
using SuscripcionesYContratos.Aplicacion.CalendarioEntrega.ListarCalendarioEntrega;
using Xunit;

namespace SuscripcionesYContratos.ApiTests;

public sealed class CalendarioEntregaControllerTests
{
    [Fact]
    public async Task CrearCalendarioEntrega_LlamaMediatorSend_ConElMismoCommand_Y_DevuelveOk()
    {
        // Arrange
        var mediator = new Mock<IMediator>(MockBehavior.Strict);

        var command = new CrearCalendarioEntregaCommand(
            contratoId: Guid.NewGuid(),
            fechaEntrega: DateOnly.FromDateTime(DateTime.Today),
            horaEntrega: TimeOnly.FromDateTime(DateTime.Now));

        var expected = Result<Guid>.Success(Guid.NewGuid());

        var ct = new CancellationTokenSource().Token;

        mediator
            .Setup(m => m.Send(command, ct))
            .ReturnsAsync(expected);

        var controller = new CalendarioEntregaController(mediator.Object);

        // Act
        var result = await controller.CrearCalendarioEntrega(command, ct);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);

        mediator.Verify(m => m.Send(command, ct), Times.Once);
        mediator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CrearCalendarioEntrega_PropagaCancellationToken_AlMediator()
    {
        // Arrange
        var mediator = new Mock<IMediator>(MockBehavior.Strict);

        var command = new CrearCalendarioEntregaCommand(
            contratoId: Guid.NewGuid(),
            fechaEntrega: DateOnly.FromDateTime(DateTime.Today),
            horaEntrega: TimeOnly.FromDateTime(DateTime.Now));

        var cts = new CancellationTokenSource();
        var ct = cts.Token;

        mediator
            .Setup(m => m.Send(command, ct))
            .ReturnsAsync(Result<Guid>.Success(Guid.NewGuid()));

        var controller = new CalendarioEntregaController(mediator.Object);

        // Act
        var _ = await controller.CrearCalendarioEntrega(command, ct);

        // Assert
        mediator.Verify(m => m.Send(command, ct), Times.Once);
        mediator.VerifyNoOtherCalls();
    }
}