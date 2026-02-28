using Joseco.DDD.Core.Results;
using MediatR;
using SuscripcionesYContratos.Aplicacion.Suscripciones.DTO;

namespace SuscripcionesYContratos.Aplicacion.Suscripciones.ObtenerSuscripcion;

public sealed record ObtenerSuscripcionQuery(Guid suscripcionId) : IRequest<Result<SuscripcionDto>>;