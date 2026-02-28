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

        public Task UpdateAsync(CalendarioEntrega calendarioEntrega)
        {
            _dbContext.Entregas.Update(calendarioEntrega);
            return Task.CompletedTask;
        }
    }
}