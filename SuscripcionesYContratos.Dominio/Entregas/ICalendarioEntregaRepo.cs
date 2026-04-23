using Joseco.DDD.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Dominio.Entregas
{
    public interface ICalendarioEntregaRepo : IRepository<CalendarioEntrega>
    {
        Task UpdateAsync(CalendarioEntrega calendarioEntrega);

        // NUEVO: listar últimos 50 (opcionalmente por contratoId y/o rango de fechas, máx 15 días)
        Task<IReadOnlyList<CalendarioEntrega>> ListarUltimosAsync(
            Guid? contratoId,
            DateOnly? desde,
            DateOnly? hasta,
            int take,
            CancellationToken cancellationToken);

        // NUEVO: obtener última entrega (por fecha/hora) de un contrato
        Task<CalendarioEntrega?> GetUltimaEntregaDeContratoAsync(
            Guid contratoId,
            CancellationToken cancellationToken);
    }
}
