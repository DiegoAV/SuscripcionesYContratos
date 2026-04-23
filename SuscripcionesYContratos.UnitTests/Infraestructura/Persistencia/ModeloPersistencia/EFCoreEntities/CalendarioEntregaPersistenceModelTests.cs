using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SuscripcionesYContratos.Infraestructura.Persistencia.ModeloPersistencia;
using SuscripcionesYContratos.Infraestructura.Persistencia.ModeloPersistencia.EFCoreEntities;
using Xunit;

namespace SuscripcionesYContratos.UnitTests.Infraestructura.Persistencia.ModeloPersistencia.EFCoreEntities;

public sealed class CalendarioEntregaPersistenceModelTests
{
    private static PersistenceDbContext CreateSqliteInMemoryContext()
    {
        // SQLite in-memory sirve para validar mapeos relacionales mejor que InMemory provider.
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<PersistenceDbContext>()
            .UseSqlite(connection)
            .Options;

        var ctx = new PersistenceDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    private static IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void DataAnnotations_ModeloValido_NoGeneraErrores()
    {
        var contrato = new ContratosPersistenceModel
        {
            Id = Guid.NewGuid(),
            pacienteId = Guid.NewGuid(),
            suscripcionId = Guid.NewGuid(),
            Suscripcion = new SuscripcionesPersistenceModel
            {
                Id = Guid.NewGuid(),
                nombre = "Plan",
                descripcion = "Desc",
                cantidadDias = 5,
                precioDia = 10m
            },
            planId = Guid.NewGuid(),
            hora = new TimeOnly(6, 30),
            inicio = new DateOnly(2026, 1, 1),
            fin = new DateOnly(2026, 1, 10),
            incluyeFinDeSemana = false,
            cantidadEntregas = 5,
            precioTotal = 50m,
            estado = 1,
            politicaCancelacionDias = 2
        };

        var model = new CalendarioEntregaPersistenceModel
        {
            Id = Guid.NewGuid(),
            contratoId = contrato.Id,
            Contrato = contrato,
            fecha = new DateOnly(2026, 1, 2),
            hora = new TimeOnly(6, 30),
            estado = 1,
            updateAt = null
        };

        var errors = ValidateModel(model);

        Assert.Empty(errors);
    }

    [Fact]
    public void DataAnnotations_ContratoNoAsignado_FallaPorRequired()
    {
        var model = new CalendarioEntregaPersistenceModel
        {
            Id = Guid.NewGuid(),
            contratoId = Guid.NewGuid(),
            Contrato = null!, // forzamos invalidación de [Required] en runtime validation
            fecha = new DateOnly(2026, 1, 2),
            hora = new TimeOnly(6, 30),
            estado = 1
        };

        var errors = ValidateModel(model);

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CalendarioEntregaPersistenceModel.Contrato)));
    }

    [Fact]
    public void EfCore_MapeaATablaCalendarioEntrega_Y_PKEsId()
    {
        using var ctx = CreateSqliteInMemoryContext();

        var entityType = ctx.Model.FindEntityType(typeof(CalendarioEntregaPersistenceModel));
        Assert.NotNull(entityType);

        Assert.Equal("CalendarioEntrega", entityType!.GetTableName());

        var pk = entityType.FindPrimaryKey();
        Assert.NotNull(pk);
        Assert.Single(pk!.Properties);
        Assert.Equal(nameof(CalendarioEntregaPersistenceModel.Id), pk.Properties[0].Name);
    }

    [Fact]
    public void EfCore_ColumnNames_SonLosEsperados()
    {
        using var ctx = CreateSqliteInMemoryContext();
        var entityType = ctx.Model.FindEntityType(typeof(CalendarioEntregaPersistenceModel))!;

        Assert.Equal("Id", entityType.FindProperty(nameof(CalendarioEntregaPersistenceModel.Id))!.GetColumnName());

        Assert.Equal("contratoId", entityType.FindProperty(nameof(CalendarioEntregaPersistenceModel.contratoId))!.GetColumnName());
        Assert.Equal("fecha", entityType.FindProperty(nameof(CalendarioEntregaPersistenceModel.fecha))!.GetColumnName());
        Assert.Equal("hora", entityType.FindProperty(nameof(CalendarioEntregaPersistenceModel.hora))!.GetColumnName());
        Assert.Equal("estado", entityType.FindProperty(nameof(CalendarioEntregaPersistenceModel.estado))!.GetColumnName());
        Assert.Equal("updateAt", entityType.FindProperty(nameof(CalendarioEntregaPersistenceModel.updateAt))!.GetColumnName());
    }

    [Fact]
    public void EfCore_PropiedadesRequired_SonNoNulas_EnElModelo()
    {
        using var ctx = CreateSqliteInMemoryContext();
        var entityType = ctx.Model.FindEntityType(typeof(CalendarioEntregaPersistenceModel))!;

        Assert.False(entityType.FindProperty(nameof(CalendarioEntregaPersistenceModel.contratoId))!.IsNullable);
        Assert.False(entityType.FindProperty(nameof(CalendarioEntregaPersistenceModel.fecha))!.IsNullable);
        Assert.False(entityType.FindProperty(nameof(CalendarioEntregaPersistenceModel.hora))!.IsNullable);
        Assert.False(entityType.FindProperty(nameof(CalendarioEntregaPersistenceModel.estado))!.IsNullable);

        // updateAt es opcional
        Assert.True(entityType.FindProperty(nameof(CalendarioEntregaPersistenceModel.updateAt))!.IsNullable);
    }
}