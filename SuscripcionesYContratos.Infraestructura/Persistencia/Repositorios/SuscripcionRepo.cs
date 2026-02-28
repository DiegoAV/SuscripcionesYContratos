using Microsoft.EntityFrameworkCore;
using SuscripcionesYContratos.Dominio.Suscripcion;
using SuscripcionesYContratos.Infraestructura.Persistencia.ModeloDominio;

namespace SuscripcionesYContratos.Infraestructura.Persistencia.Repositorios
{
    internal sealed class SuscripcionRepo : ISuscripcionesRepo
    {
        private readonly DomainDbContext _dbContext;

        public SuscripcionRepo(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task AddAsync(Suscripciones entity)
        {
            _dbContext.Suscripciones.Add(entity);
            return Task.CompletedTask;
        }

        public async Task<Suscripciones?> GetByIdAsync(Guid id, bool readOnly = false)
        {
            var query = _dbContext.Suscripciones.AsQueryable();

            if (readOnly)
                query = query.AsNoTracking();

            return await query.SingleOrDefaultAsync(x => x.Id == id);
        }

        public Task UpdateAsync(Suscripciones suscripcion)
        {
            _dbContext.Suscripciones.Update(suscripcion);
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<Suscripciones>> ListAsync(bool readOnly = false, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Suscripciones.AsQueryable();

            if (readOnly)
                query = query.AsNoTracking();

            return await query
                .OrderBy(x => x.nombre)
                .ToListAsync(cancellationToken);
        }
    }
}
