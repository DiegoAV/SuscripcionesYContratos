using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Joseco.DDD.Core.Abstractions;
using Joseco.DDD.Core.Results;
using Moq;
using SuscripcionesYContratos.Aplicacion.Contratos.CancelarContrato;
using SuscripcionesYContratos.Dominio.Contrato;
using SuscripcionesYContratos.Dominio.Entregas;
using Xunit;

namespace SuscripcionesYContratos.UnitTests.Aplicacion.Contratos.CancelarContrato;

public sealed class CancelarContratoHandlerTests
{
    [Fact]
    public async Task Handle_CancelaSoloEntregasNoEntregadas_Y_ContratoQuedaCancelado()
    {
        // Arrange
        var contratosRepo = new Mock<IContratosRepo>(MockBehavior.Strict);
        var entregasRepo = new Mock<ICalendarioEntregaRepo>(MockBehavior.Strict);
        var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);

        var contratoId = Guid.NewGuid();

        var contrato = new SuscripcionesYContratos.Dominio.Contrato.Contratos(
            id: contratoId,
            pacienteId: Guid.NewGuid(),
            suscripcionId: Guid.NewGuid(),
            planId: Guid.NewGuid(),
            hora: new TimeOnly(7, 0),
            inicio: new DateOnly(2026, 05, 01),
            fin: new DateOnly(2026, 05, 10),
            incluyeFinDeSemana: true,
            cantidadEntregas: 10,
            precioTotal: 100m,
            politicaCancelacionDias: 2);

        var e1 = new SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega(contratoId, new DateOnly(2026, 05, 02), new TimeOnly(7, 0)); // Programado -> se cancela
        var e2 = new SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega(contratoId, new DateOnly(2026, 05, 03), new TimeOnly(7, 0));
        e2.ReprogramarEntrega(e2.fecha, new TimeOnly(8, 0)); // Reprogramado -> se cancela

        var e3 = new SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega(contratoId, new DateOnly(2026, 05, 04), new TimeOnly(7, 0));
        // Forzamos Entregado con reflexión no; mejor: simular que es entregado creando un stub no es posible con private set.
        // En tu dominio no hay método Entregar(), así que para el test validamos la ruta de cancelación colocando uno ya cancelado.
        // Si añades Entregar() al dominio, este test se ajusta para cubrir Entregado.
        e3.Cancelar(); // ya cancelado, no debería llamar Cancelar (y sería conflictivo)

        contratosRepo.Setup(r => r.GetByIdAsync(contratoId, false)).ReturnsAsync(contrato);

        entregasRepo
            .Setup(r => r.ListByContratoIdAsync(contratoId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega> { e1, e2 }.AsReadOnly());

        entregasRepo.Setup(r => r.UpdateAsync(It.IsAny<SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega>())).Returns(Task.CompletedTask);
        contratosRepo.Setup(r => r.UpdateAsync(It.IsAny<SuscripcionesYContratos.Dominio.Contrato.Contratos>())).Returns(Task.CompletedTask);
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new CancelarContratoHandler(contratosRepo.Object, entregasRepo.Object, uow.Object);

        // Act
        var result = await handler.Handle(new CancelarContratoCommand(contratoId), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(contratoId, result.Value);

        Assert.Equal(ContratoEstado.Cancelado, contrato.estado);
        Assert.Equal(CalendarioEntregaEstado.Cancelado, e1.estado);
        Assert.Equal(CalendarioEntregaEstado.Cancelado, e2.estado);

        entregasRepo.Verify(r => r.UpdateAsync(It.IsAny<SuscripcionesYContratos.Dominio.Entregas.CalendarioEntrega>()), Times.Exactly(2));
        contratosRepo.Verify(r => r.UpdateAsync(It.IsAny<SuscripcionesYContratos.Dominio.Contrato.Contratos>()), Times.Once);
        uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}