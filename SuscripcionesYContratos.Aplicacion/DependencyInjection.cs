using Microsoft.Extensions.DependencyInjection;
using SuscripcionesYContratos.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Aplicacion
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAplicacion(this IServiceCollection services)
        {
            // Aquí puedes registrar tus servicios de la capa de aplicación, como handlers, servicios de dominio, etc.
            // Por ejemplo:
            // services.AddScoped<ISuscripcionesService, SuscripcionesService>();
            // services.AddScoped<IContratosService, ContratosService>();
            services.AddDominio();
            services.AddMediatR(cfg => 
            { 
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            return services;
        }
    }
}
