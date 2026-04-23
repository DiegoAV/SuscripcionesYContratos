using Joseco.DDD.Core.Results;
using MediatR;
using System;
using System.Collections.Generic;

namespace SuscripcionesYContratos.Aplicacion.CalendarioEntrega.ListarCalendarioEntrega;

public sealed record ListarCalendarioEntregaQuery(
    Guid? contratoId,
    DateOnly? desde,
    DateOnly? hasta) : IRequest<Result<IReadOnlyList<CalendarioEntregaDto>>>;