using Microsoft.Extensions.DependencyInjection;

namespace SuscripcionesYContratos.Dominio
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDominio(this IServiceCollection services)
        {
            // Aquí puedes registrar servicios específicos del dominio, como repositorios, servicios de dominio, etc.
            // Por ejemplo:
            // services.AddScoped<IContratosRepo, ContratosRepo>();
            return services;
        }
    }
}
