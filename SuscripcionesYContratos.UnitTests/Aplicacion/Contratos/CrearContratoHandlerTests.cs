using Joseco.DDD.Core.Abstractions;
using Moq;
using SuscripcionesYContratos.Aplicacion.Contratos.CrearContrato;
using SuscripcionesYContratos.Dominio.Contrato;
using SuscripcionesYContratos.Dominio.Entregas;
using SuscripcionesYContratos.Dominio.Suscripcion;
using Xunit;

namespace SuscripcionesYContratos.UnitTests.Aplicacion.Contratos;

public sealed partial class CrearContratoHandlerTests
{
    [Fact]
    public async Task Handle_SiCantidadDiasSuscripcionEsCero_RetornaFailure()
    {
        // Arrange
        var contratosRepo = new Mock<IContratosRepo>(MockBehavior.Strict);
        var suscripcionesRepo = new Mock<ISuscripcionesRepo>(MockBehavior.Strict);
        var calendarioRepo = new Mock<ICalendarioEntregaRepo>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);

        var suscripcionId = Guid.NewGuid();
        var suscripcion = new SuscripcionesYContratos.Dominio.Suscripcion.Suscripciones(
            suscripcionID: suscripcionId,
            nombre: "Plan",
            descripcion: "Desc",
            cantidadDias: 0,
            precioDia: 10m);

        suscripcionesRepo
            .Setup(x => x.GetByIdAsync(suscripcionId, true))
            .ReturnsAsync(suscripcion);

        var handler = new CrearContratoHandler(
            contratosRepo.Object,
            suscripcionesRepo.Object,
            calendarioRepo.Object,
            unitOfWork.Object);

        var command = new CrearContratoCommand(
            pacienteId: Guid.NewGuid(),
            suscripcionId: suscripcionId,
            planId: Guid.NewGuid(),
            hora: new TimeOnly(7, 0),
            inicio: new DateOnly(2026, 04, 06),
            incluyeFinDeSemana: false,
            politicaCancelacionDias: 2);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);

        contratosRepo.VerifyNoOtherCalls();
        calendarioRepo.VerifyNoOtherCalls();
        unitOfWork.VerifyNoOtherCalls();
        suscripcionesRepo.VerifyAll();
    }

    [Fact]
    public async Task Handle_IncluyeFinDeSemanaFalse_CalculaSoloLaborables_GeneraEntregasLaborables()
    {
        // Arrange
        var contratosRepo = new Mock<IContratosRepo>(MockBehavior.Strict);
        var suscripcionesRepo = new Mock<ISuscripcionesRepo>(MockBehavior.Strict);
        var calendarioRepo = new Mock<ICalendarioEntregaRepo>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);

        var suscripcionId = Guid.NewGuid();

        // 4 días: vie->lun: laborables=2
        var suscripcion = new SuscripcionesYContratos.Dominio.Suscripcion.Suscripciones(
            suscripcionID: suscripcionId,
            nombre: "Plan",
            descripcion: "Desc",
            cantidadDias: 4,
            precioDia: 10m);

        suscripcionesRepo
            .Setup(x => x.GetByIdAsync(suscripcionId, true))
            .ReturnsAsync(suscripcion);

        SuscripcionesYContratos.Dominio.Contrato.Contratos? contratoAgregado = null;

        contratosRepo
            .Setup(x => x.AddAsync(It.IsAny<SuscripcionesYContratos.Dominio.Contrato.Contratos>()))
            .Callback<SuscripcionesYContratos.Dominio.Contrato.Contratos>(c => contratoAgregado = c)
            .Returns(Task.CompletedTask);

        var entregasAgregadas = new List<SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega>();

        calendarioRepo
            .Setup(x => x.AddAsync(It.IsAny<SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega>()))
            .Callback<SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega>(e => entregasAgregadas.Add(e))
            .Returns(Task.CompletedTask);

        unitOfWork
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CrearContratoHandler(
            contratosRepo.Object,
            suscripcionesRepo.Object,
            calendarioRepo.Object,
            unitOfWork.Object);

        var inicio = new DateOnly(2026, 04, 10); // viernes
        var command = new CrearContratoCommand(
            pacienteId: Guid.NewGuid(),
            suscripcionId: suscripcionId,
            planId: Guid.NewGuid(),
            hora: new TimeOnly(7, 0),
            inicio: inicio,
            incluyeFinDeSemana: false,
            politicaCancelacionDias: 2);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(contratoAgregado);

        Assert.Equal(inicio.AddDays(3), contratoAgregado!.fin);
        Assert.Equal(2, contratoAgregado.cantidadEntregas);
        Assert.Equal(20m, contratoAgregado.precioTotal);

        Assert.Equal(2, entregasAgregadas.Count);
        Assert.DoesNotContain(entregasAgregadas, e => e.fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);

        unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));

        suscripcionesRepo.VerifyAll();
        contratosRepo.VerifyAll();
        calendarioRepo.VerifyAll();
        unitOfWork.VerifyAll();
    }
}
