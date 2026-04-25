using Microsoft.EntityFrameworkCore;
using SuscripcionesYContratos.Dominio.Entregas;
using SuscripcionesYContratos.Infraestructura.Persistencia.ModeloDominio;

namespace SuscripcionesYContratos.Infraestructura.Persistencia.Repositorios
{
    internal sealed class CalendarioEntregaRepo : ICalendarioEntregaRepo
    {
        private readonly DomainDbContext _dbContext;

        public CalendarioEntregaRepo(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task AddAsync(CalendarioEntrega entity)
        {
            _dbContext.Entregas.Add(entity);
            return Task.CompletedTask;
        }

        public async Task<CalendarioEntrega?> GetByIdAsync(Guid id, bool readOnly = false)
        {
            var query = _dbContext.Entregas.AsQueryable();

            if (readOnly)
                query = query.AsNoTracking();

            return await query.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<CalendarioEntrega>> ListarUltimosAsync(
            Guid? contratoId,
            DateOnly? desde,
            DateOnly? hasta,
            int take,
            CancellationToken cancellationToken)
        {
            var query = _dbContext.Entregas.AsNoTracking().AsQueryable();

            if (contratoId.HasValue)
                query = query.Where(x => x.contratoId == contratoId.Value);

            if (desde.HasValue)
                query = query.Where(x => x.fecha >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(x => x.fecha <= hasta.Value);

            query = query
                .OrderByDescending(x => x.fecha)
                .ThenByDescending(x => x.hora)
                .Take(take);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<CalendarioEntrega?> GetUltimaEntregaDeContratoAsync(
            Guid contratoId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Entregas
                .AsNoTracking()
                .Where(x => x.contratoId == contratoId)
                .OrderByDescending(x => x.fecha)
                .ThenByDescending(x => x.hora)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<CalendarioEntrega>> ListByContratoIdAsync(
            Guid contratoId,
            bool readOnly = false,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Entregas.AsQueryable();

            if (readOnly)
                query = query.AsNoTracking();

            return await query
                .Where(x => x.contratoId == contratoId)
                .OrderBy(x => x.fecha)
                .ThenBy(x => x.hora)
                .ToListAsync(cancellationToken);
        }

        public Task UpdateAsync(CalendarioEntrega calendarioEntrega)
        {
            _dbContext.Entregas.Update(calendarioEntrega);
            return Task.CompletedTask;
        }
    }
}