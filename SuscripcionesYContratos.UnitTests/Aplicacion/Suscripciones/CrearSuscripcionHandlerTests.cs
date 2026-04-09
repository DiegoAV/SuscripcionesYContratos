using Joseco.DDD.Core.Abstractions;
using Moq;
using SuscripcionesYContratos.Aplicacion.Suscripciones.CrearSuscripcion;
using SuscripcionesYContratos.Dominio.Suscripcion;
using Xunit;

namespace SuscripcionesYContratos.UnitTests.Aplicacion.Suscripciones;

public sealed class CrearSuscripcionHandlerTests
{
    [Fact]
    public async Task Handle_CreaSuscripcion_GuardaYHaceCommit_RetornaSuccessConId()
    {
        // Arrange
        var repo = new Mock<ISuscripcionesRepo>(MockBehavior.Strict);
        var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);

        SuscripcionesYContratos.Dominio.Suscripcion.Suscripciones? agregada = null;

        repo.Setup(x => x.AddAsync(It.IsAny<SuscripcionesYContratos.Dominio.Suscripcion.Suscripciones>()))
            .Callback<SuscripcionesYContratos.Dominio.Suscripcion.Suscripciones>(s => agregada = s)
            .Returns(Task.CompletedTask);

        uow.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var handler = new SuscripcionesYContratos.Aplicacion.Suscripciones.CrearSuscripcion.CrearSuscripcionHandler(repo.Object, uow.Object);

        var cmd = new CrearSuscripcionCommand(
            nombre: "Plan A",
            descripcion: "Desc",
            cantidadDias: 5,
            precioDia: 10m);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(agregada);
        Assert.Equal(agregada!.Id, result.Value);

        repo.Verify(x => x.AddAsync(It.IsAny<SuscripcionesYContratos.Dominio.Suscripcion.Suscripciones>()), Times.Once);
        uow.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

        repo.VerifyAll();
        uow.VerifyAll();
    }
}
