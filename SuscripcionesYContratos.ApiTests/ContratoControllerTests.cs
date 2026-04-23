using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Joseco.DDD.Core.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SuscripcionesYContratos.API.Controllers;
using SuscripcionesYContratos.Aplicacion.Contratos.CrearContrato;
using SuscripcionesYContratos.Aplicacion.Contratos.DTO;
using SuscripcionesYContratos.Aplicacion.Contratos.ListarContratos;
using SuscripcionesYContratos.Aplicacion.Contratos.ListarContratosXEstado;
using SuscripcionesYContratos.Aplicacion.Contratos.ListarContratosXPaciente;
using Xunit;

namespace SuscripcionesYContratos.ApiTests;

public sealed class ContratoControllerTests
{
    [Fact]
    public async Task CrearContrato_LlamaMediatorSend_ConElMismoCommand_Y_DevuelveOk()
    {
        // Arrange
        var mediator = new Mock<IMediator>(MockBehavior.Strict);

        var command = new CrearContratoCommand(
            pacienteId: Guid.NewGuid(),
            suscripcionId: Guid.NewGuid(),
            planId: Guid.NewGuid(),
            hora: new TimeOnly(7, 0),
            inicio: new DateOnly(2026, 04, 06),
            incluyeFinDeSemana: false,
            politicaCancelacionDias: 2);

        var expected = Result<Guid>.Success(Guid.NewGuid());

        var ct = new CancellationTokenSource().Token;

        mediator
            .Setup(m => m.Send(command, ct))
            .ReturnsAsync(expected);

        var controller = new ContratoController(mediator.Object);

        // Act
        var result = await controller.CrearContrato(command, ct);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);

        mediator.Verify(m => m.Send(command, ct), Times.Once);
        mediator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CrearContrato_PropagaCancellationToken_AlMediator()
    {
        // Arrange
        var mediator = new Mock<IMediator>(MockBehavior.Strict);

        var command = new CrearContratoCommand(
            pacienteId: Guid.NewGuid(),
            suscripcionId: Guid.NewGuid(),
            planId: Guid.NewGuid(),
            hora: new TimeOnly(7, 0),
            inicio: new DateOnly(2026, 04, 06),
            incluyeFinDeSemana: false,
            politicaCancelacionDias: 2);

        var cts = new CancellationTokenSource();
        var ct = cts.Token;

        mediator
            .Setup(m => m.Send(command, ct))
            .ReturnsAsync(Result<Guid>.Success(Guid.NewGuid()));

        var controller = new ContratoController(mediator.Object);

        // Act
        var _ = await controller.CrearContrato(command, ct);

        // Assert
        mediator.Verify(m => m.Send(command, ct), Times.Once);
        mediator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ListarContratos_ConstruyeQuery_Y_LlamaMediator_Y_DevuelveOk()
    {
        // Arrange
        var mediator = new Mock<IMediator>(MockBehavior.Strict);

        ListarContratosQuery? captured = null;

        // Ojo: en tu solución el handler/query devuelve Result<ContratoDto[]> (array),
        // no Result<IReadOnlyList<ContratoDto>>.
        var expected = Result<IReadOnlyList<ContratoDto>>.Success(Array.Empty<ContratoDto>() as IReadOnlyList<ContratoDto>);

        var ct = new CancellationTokenSource().Token;

        mediator
            .Setup(m => m.Send(It.IsAny<ListarContratosQuery>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((q, _) => captured = (ListarContratosQuery)q)
            .Returns(Task.FromResult(expected));

        var controller = new ContratoController(mediator.Object);

        // Act
        var result = await controller.ListarContratos(ct);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);

        Assert.NotNull(captured);
        mediator.Verify(m => m.Send(It.IsAny<ListarContratosQuery>(), ct), Times.Once);
        mediator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ListarContratosXPaciente_UsaPacienteIdDeRuta_Y_LlamaMediator_Y_DevuelveOk()
    {
        // Arrange
        var mediator = new Mock<IMediator>(MockBehavior.Strict);

        ListarContratosXPacienteQuery? captured = null;

        var pacienteId = Guid.NewGuid();
        var expected = Result<IReadOnlyList<ContratoDto>>.Success(Array.Empty<ContratoDto>() as IReadOnlyList<ContratoDto>);

        var ct = new CancellationTokenSource().Token;

        mediator
            .Setup(m => m.Send(It.IsAny<ListarContratosXPacienteQuery>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((q, _) => captured = (ListarContratosXPacienteQuery)q)
            .Returns(Task.FromResult(expected));

        var controller = new ContratoController(mediator.Object);

        // Act
        var result = await controller.ListarContratosXPaciente(pacienteId, ct);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);

        Assert.NotNull(captured);
        Assert.Equal(pacienteId, captured!.pacienteId);

        mediator.Verify(m => m.Send(It.IsAny<ListarContratosXPacienteQuery>(), ct), Times.Once);
        mediator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ListarContratosXEstado_UsaEstadoDeRuta_Y_LlamaMediator_Y_DevuelveOk()
    {
        // Arrange
        var mediator = new Mock<IMediator>(MockBehavior.Strict);

        ListarContratosXEstadoQuery? captured = null;

        var estado = 2;
        var expected = Result<IReadOnlyList<ContratoDto>>.Success(Array.Empty<ContratoDto>() as IReadOnlyList<ContratoDto>);

        var ct = new CancellationTokenSource().Token;

        mediator
            .Setup(m => m.Send(It.IsAny<ListarContratosXEstadoQuery>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((q, _) => captured = (ListarContratosXEstadoQuery)q)
            .Returns(Task.FromResult(expected));

        var controller = new ContratoController(mediator.Object);

        // Act
        var result = await controller.ListarContratosXEstado(estado, ct);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);

        Assert.NotNull(captured);
        Assert.Equal(estado, captured!.estado);

        mediator.Verify(m => m.Send(It.IsAny<ListarContratosXEstadoQuery>(), ct), Times.Once);
        mediator.VerifyNoOtherCalls();
    }
}