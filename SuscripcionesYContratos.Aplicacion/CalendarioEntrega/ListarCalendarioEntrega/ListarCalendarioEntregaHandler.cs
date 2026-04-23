using Joseco.DDD.Core.Results;
using MediatR;
using SuscripcionesYContratos.Dominio.Entregas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Aplicacion.CalendarioEntrega.ListarCalendarioEntrega;

public sealed class ListarCalendarioEntregaHandler
    : IRequestHandler<ListarCalendarioEntregaQuery, Result<IReadOnlyList<CalendarioEntregaDto>>>
{
    private const int MaxDaysRange = 15;
    private const int Take = 50;

    private readonly ICalendarioEntregaRepo _repo;

    public ListarCalendarioEntregaHandler(ICalendarioEntregaRepo repo)
    {
        _repo = repo;
    }

    public async Task<Result<IReadOnlyList<CalendarioEntregaDto>>> Handle(
        ListarCalendarioEntregaQuery request,
        CancellationToken cancellationToken)
    {
        if (request.desde.HasValue && request.hasta.HasValue)
        {
            if (request.hasta.Value < request.desde.Value)
                return Result.Failure<IReadOnlyList<CalendarioEntregaDto>>(
                    CalendarioEntregaError.CalendarioEntregaInvalido);

            var days = request.hasta.Value.DayNumber - request.desde.Value.DayNumber;
            if (days > MaxDaysRange)
                return Result.Failure<IReadOnlyList<CalendarioEntregaDto>>(
                    CalendarioEntregaError.CalendarioEntregaInvalido);
        }

        var entregas = await _repo.ListarUltimosAsync(
            contratoId: request.contratoId,
            desde: request.desde,
            hasta: request.hasta,
            take: Take,
            cancellationToken: cancellationToken);

        var dtos = entregas
            .Select(e => new CalendarioEntregaDto(
                Id: e.Id,
                contratoId: e.contratoId,
                fecha: e.fecha,
                hora: e.hora,
                estado: (int)e.estado,
                updateAt: e.updateAt))
            .ToArray();

        return Result.Success<IReadOnlyList<CalendarioEntregaDto>>(dtos);
    }
}