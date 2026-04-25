using Joseco.DDD.Core.Results;
using MediatR;

namespace SuscripcionesYContratos.Aplicacion.Contratos.CancelarContrato;

public sealed record CancelarContratoCommand(Guid contratoId) : IRequest<Result<Guid>>;