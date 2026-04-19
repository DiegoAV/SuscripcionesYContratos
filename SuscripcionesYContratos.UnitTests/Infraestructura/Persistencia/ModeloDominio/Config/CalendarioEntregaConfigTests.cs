using System;
using Joseco.DDD.Core.Abstractions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.InMemory; // <-- Agrega esta línea
using SuscripcionesYContratos.Dominio.Entregas;
using SuscripcionesYContratos.Infraestructura.Persistencia.ModeloDominio;
using Xunit;

namespace SuscripcionesYContratos.UnitTests.Infraestructura.Persistencia.ModeloDominio.Config;

public sealed class CalendarioEntregaConfigTests
{
    private static DomainDbContext CreateSqliteInMemoryContext()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<DomainDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        var ctx = new DomainDbContext(options);
        ctx.Database.EnsureCreated(); // fuerza el modelo y la creación de schema en SQLite

        return ctx;
    }

    [Fact]
    public void Modelo_CalendarioEntrega_MapeaATablaCalendarioEntrega_YTienePK()
    {
        using var ctx = CreateSqliteInMemoryContext();

        var entityType = ctx.Model.FindEntityType(typeof(CalendarioEntrega));
        Assert.NotNull(entityType);

        Assert.Equal("CalendarioEntrega", entityType!.GetTableName());

        var pk = entityType.FindPrimaryKey();
        Assert.NotNull(pk);
        Assert.Single(pk!.Properties);
        Assert.Equal(nameof(AggregateRoot.Id), pk.Properties[0].Name); // "Id"
    }

    [Fact]
    public void Modelo_CalendarioEntrega_PropiedadesRequeridas_EstanMarcadasComoNoNull()
    {
        using var ctx = CreateSqliteInMemoryContext();

        var entityType = ctx.Model.FindEntityType(typeof(CalendarioEntrega))!;
        Assert.False(entityType.FindProperty(nameof(CalendarioEntrega.contratoId))!.IsNullable);
        Assert.False(entityType.FindProperty(nameof(CalendarioEntrega.fecha))!.IsNullable);
        Assert.False(entityType.FindProperty(nameof(CalendarioEntrega.hora))!.IsNullable);
        Assert.False(entityType.FindProperty(nameof(CalendarioEntrega.estado))!.IsNullable);
    }

    [Fact]
    public void Modelo_CalendarioEntrega_IgnoraDomainEvents_YCampoDomainEvents()
    {
        using var ctx = CreateSqliteInMemoryContext();

        var entityType = ctx.Model.FindEntityType(typeof(CalendarioEntrega))!;

        // Ignorados por configuración: no deben aparecer como propiedades mapeadas
        Assert.Null(entityType.FindProperty("DomainEvents"));
        Assert.Null(entityType.FindProperty("_domainEvents"));
    }
}