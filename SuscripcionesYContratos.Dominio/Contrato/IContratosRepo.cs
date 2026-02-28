using Joseco.DDD.Core.Abstractions;

namespace SuscripcionesYContratos.Dominio.Contrato
{
    public interface IContratosRepo : IRepository<Contratos>
    {
        Task UpdateAsync(Contratos contrato);

        Task<IReadOnlyList<Contratos>> ListAsync(bool readOnly = false, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Contratos>> ListByPacienteIdAsync(Guid pacienteId, bool readOnly = false, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Contratos>> ListByEstadoAsync(ContratoEstado estado, bool readOnly = false, CancellationToken cancellationToken = default);
    }
}
