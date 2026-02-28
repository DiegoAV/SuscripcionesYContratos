using Microsoft.EntityFrameworkCore;
using SuscripcionesYContratos.Dominio.Contrato;
using SuscripcionesYContratos.Infraestructura.Persistencia.ModeloDominio;

namespace SuscripcionesYContratos.Infraestructura.Persistencia.Repositorios;

internal sealed class ContratoRepo : IContratosRepo
{
    private readonly DomainDbContext _dbContext;

    public ContratoRepo(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Contratos entity)
    {
        _dbContext.Contratos.Add(entity);
        return Task.CompletedTask;
    }

    public async Task<Contratos?> GetByIdAsync(Guid id, bool readOnly = false)
    {
        var query = _dbContext.Contratos.AsQueryable();

        if (readOnly)
            query = query.AsNoTracking();

        return await query.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<Contratos>> ListAsync(bool readOnly = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Contratos.AsQueryable();

        if (readOnly)
            query = query.AsNoTracking();

        return await query
            .OrderByDescending(x => x.inicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Contratos>> ListByPacienteIdAsync(Guid pacienteId, bool readOnly = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Contratos.AsQueryable();

        if (readOnly)
            query = query.AsNoTracking();

        return await query
            .Where(x => x.pacienteId == pacienteId)
            .OrderByDescending(x => x.inicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Contratos>> ListByEstadoAsync(ContratoEstado estado, bool readOnly = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Contratos.AsQueryable();

        if (readOnly)
            query = query.AsNoTracking();

        return await query
            .Where(x => x.estado == estado)
            .OrderByDescending(x => x.inicio)
            .ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(Contratos contrato)
    {
        _dbContext.Contratos.Update(contrato);
        return Task.CompletedTask;
    }
}