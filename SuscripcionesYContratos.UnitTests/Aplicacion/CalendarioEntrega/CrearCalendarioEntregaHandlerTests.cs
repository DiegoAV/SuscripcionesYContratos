using Joseco.DDD.Core.Abstractions;
using Moq;
using SuscripcionesYContratos.Aplicacion.CalendarioEntrega.CrearCalendarioEntrega;
using SuscripcionesYContratos.Dominio.Contrato;
using SuscripcionesYContratos.Dominio.Entregas;
using Xunit;

namespace SuscripcionesYContratos.UnitTests.Aplicacion.CalendarioEntrega;

public sealed class CrearCalendarioEntregaHandlerTests
{
    [Fact]
    public async Task Handle_SiContratoNoExiste_RetornaFailure_YNoAgregaEntregas()
    {
        // Arrange
        var calendarioRepo = new Mock<ICalendarioEntregaRepo>(MockBehavior.Strict);
        var contratosRepo = new Mock<IContratosRepo>(MockBehavior.Strict);
        var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);

        contratosRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), true))
            .ReturnsAsync((SuscripcionesYContratos.Dominio.Contrato.Contratos?)null);

        var handler = new CrearCalendarioEntregaHandler(calendarioRepo.Object, contratosRepo.Object, uow.Object);

        var cmd = new CrearCalendarioEntregaCommand(
            contratoId: Guid.NewGuid(),
            fechaEntrega: new DateOnly(2026, 04, 06),
            horaEntrega: new TimeOnly(7, 0));

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        calendarioRepo.VerifyNoOtherCalls();
        uow.VerifyNoOtherCalls();
        contratosRepo.VerifyAll();
    }

    [Fact]
    public async Task Handle_IncluyeFinDeSemanaTrue_CreaUnaEntregaPorDia()
    {
        // Arrange
        var calendarioRepo = new Mock<ICalendarioEntregaRepo>(MockBehavior.Strict);
        var contratosRepo = new Mock<IContratosRepo>(MockBehavior.Strict);
        var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);

        var contratoId = Guid.NewGuid();
        var inicio = new DateOnly(2026, 04, 06); // lunes
        var fin = inicio.AddDays(2);             // 3 días

        var contrato = new SuscripcionesYContratos.Dominio.Contrato.Contratos(
            id: contratoId,
            pacienteId: Guid.NewGuid(),
            suscripcionId: Guid.NewGuid(),
            planId: Guid.NewGuid(),
            hora: new TimeOnly(7, 0),
            inicio: inicio,
            fin: fin,
            incluyeFinDeSemana: true,
            cantidadEntregas: 3,
            precioTotal: 30m,
            politicaCancelacionDias: 2);

        contratosRepo
            .Setup(x => x.GetByIdAsync(contratoId, true))
            .ReturnsAsync(contrato);

        var entregas = new List<SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega>();

        calendarioRepo
            .Setup(x => x.AddAsync(It.IsAny<SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega>()))
            .Callback<SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega>(e => entregas.Add(e))
            .Returns(Task.CompletedTask);

        var handler = new CrearCalendarioEntregaHandler(calendarioRepo.Object, contratosRepo.Object, uow.Object);

        var cmd = new CrearCalendarioEntregaCommand(
            contratoId: contratoId,
            fechaEntrega: inicio,                 // no se usa en el handler, pero es requerido por el record
            horaEntrega: new TimeOnly(7, 0));     // no se usa en el handler, pero es requerido por el record

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(contratoId, result.Value);

        Assert.Equal(3, entregas.Count);
        calendarioRepo.Verify(x => x.AddAsync(It.IsAny<SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega>()), Times.Exactly(3));

        contratosRepo.VerifyAll();
        calendarioRepo.VerifyAll();
        uow.VerifyNoOtherCalls(); // este handler no hace commit
    }

    [Fact]
    public async Task Handle_IncluyeFinDeSemanaFalse_NoCreaEntregaEnSabadoYDomingo()
    {
        // Arrange
        var calendarioRepo = new Mock<ICalendarioEntregaRepo>(MockBehavior.Strict);
        var contratosRepo = new Mock<IContratosRepo>(MockBehavior.Strict);
        var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);

        var contratoId = Guid.NewGuid();

        // Rango: viernes->lunes (4 días): vie, sáb, dom, lun => laborables = 2
        var inicio = new DateOnly(2026, 04, 10); // viernes
        var fin = inicio.AddDays(3);             // lunes

        var contrato = new SuscripcionesYContratos.Dominio.Contrato.Contratos(
            id: contratoId,
            pacienteId: Guid.NewGuid(),
            suscripcionId: Guid.NewGuid(),
            planId: Guid.NewGuid(),
            hora: new TimeOnly(7, 0),
            inicio: inicio,
            fin: fin,
            incluyeFinDeSemana: false,
            cantidadEntregas: 2,
            precioTotal: 20m,
            politicaCancelacionDias: 2);

        contratosRepo
            .Setup(x => x.GetByIdAsync(contratoId, true))
            .ReturnsAsync(contrato);

        var entregas = new List<SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega>();

        calendarioRepo
            .Setup(x => x.AddAsync(It.IsAny<SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega>()))
            .Callback<SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega>(e => entregas.Add(e))
            .Returns(Task.CompletedTask);

        var handler = new CrearCalendarioEntregaHandler(calendarioRepo.Object, contratosRepo.Object, uow.Object);

        var cmd = new CrearCalendarioEntregaCommand(
            contratoId: contratoId,
            fechaEntrega: inicio,
            horaEntrega: new TimeOnly(7, 0));

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, entregas.Count);
        Assert.DoesNotContain(entregas, e => e.fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);

        calendarioRepo.Verify(x => x.AddAsync(It.IsAny<SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega>()), Times.Exactly(2));
        contratosRepo.VerifyAll();
        calendarioRepo.VerifyAll();
        uow.VerifyNoOtherCalls();
    }
}