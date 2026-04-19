using Joseco.DDD.Core.Results;
using Moq;
using SuscripcionesYContratos.Aplicacion.Contratos.ListarContratos;
using SuscripcionesYContratos.Aplicacion.Contratos.DTO;
using SuscripcionesYContratos.Dominio.Contrato;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SuscripcionesYContratos.UnitTests.Aplicacion.Contratos;

public sealed class ListarContratosHandlerTests
{
    [Fact]
    public async Task Handle_CuandoRepoDevuelveVacio_RetornaSuccess_ConListaVaciaReadOnly_YLlamaRepoConReadOnlyTrue()
    {
        // Arrange
        var repo = new Mock<IContratosRepo>(MockBehavior.Strict);

        repo.Setup(r => r.ListAsync(true, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IReadOnlyList<SuscripcionesYContratos.Dominio.Contrato.Contratos>>(Array.Empty<SuscripcionesYContratos.Dominio.Contrato.Contratos>()));

        var handler = new ListarContratosHandler(repo.Object);

        // Act
        var result = await handler.Handle(new ListarContratosQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
        Assert.IsAssignableFrom<IReadOnlyList<ContratoDto>>(result.Value);

        repo.Verify(r => r.ListAsync(true, It.IsAny<CancellationToken>()), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_CuandoRepoDevuelveItems_MapeaTodasLasPropiedades_YPersisteOrden()
    {
        // Arrange
        var repo = new Mock<IContratosRepo>(MockBehavior.Strict);

        var c1 = new SuscripcionesYContratos.Dominio.Contrato.Contratos(
            id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
            pacienteId: Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            suscripcionId: Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            planId: Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            hora: new TimeOnly(7, 0),
            inicio: new DateOnly(2026, 04, 10),
            fin: new DateOnly(2026, 04, 12),
            incluyeFinDeSemana: false,
            cantidadEntregas: 2,
            precioTotal: 20m,
            politicaCancelacionDias: 2);

        var c2 = new SuscripcionesYContratos.Dominio.Contrato.Contratos(
            id: Guid.Parse("22222222-2222-2222-2222-222222222222"),
            pacienteId: Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
            suscripcionId: Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
            planId: Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
            hora: new TimeOnly(6, 30),
            inicio: new DateOnly(2026, 04, 13),
            fin: new DateOnly(2026, 04, 13),
            incluyeFinDeSemana: true,
            cantidadEntregas: 1,
            precioTotal: 10m,
            politicaCancelacionDias: 1);

        c2.SetHora(new TimeOnly(6, 30));
        c2.SetPoliticaCancelacionDias(1);

        repo.Setup(r => r.ListAsync(true, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IReadOnlyList<SuscripcionesYContratos.Dominio.Contrato.Contratos>>(new[] { c1, c2 }));

        var handler = new ListarContratosHandler(repo.Object);

        // Act
        var result = await handler.Handle(new ListarContratosQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);

        Assert.Equal(c1.Id, result.Value[0].Id);
        Assert.Equal(c2.Id, result.Value[1].Id);

        var dto1 = result.Value[0];
        Assert.Equal(c1.Id, dto1.Id);
        Assert.Equal(c1.pacienteId, dto1.pacienteId);
        Assert.Equal(c1.suscripcionId, dto1.suscripcionId);
        Assert.Equal(c1.planId, dto1.planId);
        Assert.Equal(c1.hora, dto1.hora);
        Assert.Equal(c1.inicio, dto1.inicio);
        Assert.Equal(c1.fin, dto1.fin);
        Assert.Equal(c1.incluyeFinDeSemana, dto1.incluyeFinDeSemana);
        Assert.Equal(c1.cantidadEntregas, dto1.cantidadEntregas);
        Assert.Equal(c1.precioTotal, dto1.precioTotal);
        Assert.Equal((int)c1.estado, dto1.estado);
        Assert.Equal(c1.politicaCancelacionDias, dto1.politicaCancelacionDias);
        Assert.Equal(c1.updateAt, dto1.updateAt);

        repo.Verify(r => r.ListAsync(true, It.IsAny<CancellationToken>()), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}
