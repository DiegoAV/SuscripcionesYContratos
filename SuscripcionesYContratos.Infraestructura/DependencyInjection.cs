using Joseco.DDD.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SuscripcionesYContratos.Aplicacion;
using SuscripcionesYContratos.Dominio.Contrato;
using SuscripcionesYContratos.Dominio.Entregas;
using SuscripcionesYContratos.Dominio.Suscripcion;
using SuscripcionesYContratos.Infraestructura.Mensajeria;
using SuscripcionesYContratos.Infraestructura.Mensajeria.planes_plan_alimetario_creado;
using SuscripcionesYContratos.Infraestructura.Outbox;
using SuscripcionesYContratos.Infraestructura.Persistencia;
using SuscripcionesYContratos.Infraestructura.Persistencia.ModeloDominio;
using SuscripcionesYContratos.Infraestructura.Persistencia.ModeloPersistencia;
using SuscripcionesYContratos.Infraestructura.Persistencia.Repositorios;
using System.Reflection;

namespace SuscripcionesYContratos.Infraestructura
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAplicacion()
                .AddPersistencia(configuration);


            services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
            /* Consumidor */
            services.AddHostedService<PlanAlimentarioCreado>();

            /* Publicador */
            services.AddHostedService<OutboxPublisherService>();

            // services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
            // services.AddHostedService<OutboxProcessorBackgroundService>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }

        private static void AddPersistencia(this IServiceCollection services, IConfiguration configuration)
        {
            var cs = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<PersistenceDbContext>(options => options.UseNpgsql(cs));
            services.AddDbContext<DomainDbContext>(options => options.UseNpgsql(cs));

            services.AddScoped<ISuscripcionesRepo, SuscripcionRepo>();
            services.AddScoped<IContratosRepo, ContratoRepo>();
            services.AddScoped<ICalendarioEntregaRepo, CalendarioEntregaRepo>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
