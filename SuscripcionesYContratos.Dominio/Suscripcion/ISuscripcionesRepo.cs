using Joseco.DDD.Core.Abstractions;

namespace SuscripcionesYContratos.Dominio.Suscripcion
{
    public interface ISuscripcionesRepo : IRepository<Suscripciones>
    {
        Task UpdateAsync(Suscripciones suscripcion);

        Task<IReadOnlyList<Suscripciones>> ListAsync(bool readOnly = false, CancellationToken cancellationToken = default);
    }
}
