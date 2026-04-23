using System;
using System.Threading;
using System.Threading.Tasks;
using Joseco.DDD.Core.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SuscripcionesYContratos.API.Controllers;
using SuscripcionesYContratos.Aplicacion.Suscripciones.ActualizarSuscripcion;
using SuscripcionesYContratos.Aplicacion.Suscripciones.CrearSuscripcion;
using SuscripcionesYContratos.Aplicacion.Suscripciones.ListarSuscripciones;
using SuscripcionesYContratos.Aplicacion.Suscripciones.DTO;
using Xunit;
using Moq.Language.Flow;

using SuscripcionDto = SuscripcionesYContratos.Aplicacion.Suscripciones.DTO.SuscripcionDto;

namespace SuscripcionesYContratos.ApiTests;

public sealed class SuscripcionControllerTests
{
    [Fact]
    public async Task CrearSuscripcion_LlamaMediatorSend_ConElMismoCommand_Y_DevuelveOk()
    {
        // Arrange
        var mediator = new Mock<IMediator>(MockBehavior.Strict);

        var command = new CrearSuscripcionCommand(
            nombre: "Plan A",
            descripcion: "Desc A",
            cantidadDias: 7,
            precioDia: 12.5m);

        var expected = Result<Guid>.Success(Guid.NewGuid());

        mediator
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var controller = new SuscripcionController(mediator.Object);

        // Act
        var result = await controller.CrearSuscripcion(command);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);

        mediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        mediator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ActualizarSuscripcion_ConstruyeCommand_DesdeBody_Y_LlamaMediator_Y_DevuelveOk()
    {
        // Arrange
        var mediator = new Mock<IMediator>(MockBehavior.Strict);

        ActualizarSuscripcionCommand? captured = null;

        var expected = Result<Guid>.Success(Guid.Parse("11111111-1111-1111-1111-111111111111"));

        mediator
            .Setup(m => m.Send(It.IsAny<ActualizarSuscripcionCommand>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((cmd, _) => captured = (ActualizarSuscripcionCommand)cmd)
            .ReturnsAsync(expected);

        var controller = new SuscripcionController(mediator.Object);

        var id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var body = new ActualizarSuscripcionBody(
            nombre: "Plan Nuevo",
            descripcion: "Desc Nueva",
            cantidadDias: 7,
            precioDia: 12.5m);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await controller.ActualizarSuscripcion(id, body, ct);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);

        Assert.NotNull(captured);
        Assert.Equal(id, captured!.suscripcionId);
        Assert.Equal(body.nombre, captured.nombre);
        Assert.Equal(body.descripcion, captured.descripcion);
        Assert.Equal(body.cantidadDias, captured.cantidadDias);
        Assert.Equal(body.precioDia, captured.precioDia);

        mediator.Verify(m => m.Send(It.IsAny<ActualizarSuscripcionCommand>(), ct), Times.Once);
        mediator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ActualizarSuscripcion_PermiteNulls_EnBody_Y_PropagaAlCommand()
    {
        // Arrange
        var mediator = new Mock<IMediator>(MockBehavior.Strict);

        ActualizarSuscripcionCommand? captured = null;

        mediator
            .Setup(m => m.Send(It.IsAny<ActualizarSuscripcionCommand>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((cmd, _) => captured = (ActualizarSuscripcionCommand)cmd)
            .ReturnsAsync(Result<Guid>.Success(Guid.NewGuid()));

        var controller = new SuscripcionController(mediator.Object);

        var id = Guid.NewGuid();
        var body = new ActualizarSuscripcionBody(
            nombre: null,
            descripcion: null,
            cantidadDias: null,
            precioDia: null);

        // Act
        var result = await controller.ActualizarSuscripcion(id, body, CancellationToken.None);

        // Assert
        Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(captured);
        Assert.Equal(id, captured!.suscripcionId);
        Assert.Null(captured.nombre);
        Assert.Null(captured.descripcion);
        Assert.Null(captured.cantidadDias);
        Assert.Null(captured.precioDia);

        mediator.Verify(m => m.Send(It.IsAny<ActualizarSuscripcionCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        mediator.VerifyNoOtherCalls();
    }
}