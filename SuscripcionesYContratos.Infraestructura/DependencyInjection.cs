using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuscripcionesYContratos.Aplicacion;
using SuscripcionesYContratos.Dominio.Suscripcion;
using SuscripcionesYContratos.Infraestructura.Persistencia.Repositorios;
using Joseco.DDD.Core.Abstractions;
using SuscripcionesYContratos.Infraestructura.Persistencia;

namespace SuscripcionesYContratos.Infraestructura
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Aquí puedes registrar tus servicios de infraestructura, como repositorios, servicios de acceso a datos, etc.
            // Por ejemplo:
            // services.AddScoped<ISuscripcionesRepository, SuscripcionesRepository>();
            // services.AddScoped<IContratosRepository, ContratosRepository>();

            services.AddAplicacion()
                .AddPersistencia(configuration);
            return services;
        }
        
        private static void AddPersistencia(this IServiceCollection services, IConfiguration configuration)
        {
            // Aquí puedes configurar tu contexto de base de datos, por ejemplo:
            // services.AddDbContext<MiDbContext>(options =>
            //     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<ISuscripcionesRepo, SuscripcionRepo>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
