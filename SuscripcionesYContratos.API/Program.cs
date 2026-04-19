using Microsoft.EntityFrameworkCore;
using SuscripcionesYContratos.Infraestructura;
using SuscripcionesYContratos.Infraestructura.Persistencia.ModeloPersistencia;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplica migraciones al arrancar (importante en Docker)
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PersistenceDbContext>();

    const int maxRetries = 10;
    for (var i = 1; i <= maxRetries; i++)
    {
        try
        {
            await db.Database.MigrateAsync();
            break;
        }
        catch (Exception) when (i < maxRetries)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
