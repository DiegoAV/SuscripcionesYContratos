using Joseco.DDD.Core.Results;
using MediatR;
using SuscripcionesYContratos.Aplicacion.Suscripciones.DTO;

namespace SuscripcionesYContratos.Aplicacion.Suscripciones.ListarSuscripciones;

public sealed record ListarSuscripcionesQuery() : IRequest<Result<IReadOnlyList<SuscripcionDto>>>;