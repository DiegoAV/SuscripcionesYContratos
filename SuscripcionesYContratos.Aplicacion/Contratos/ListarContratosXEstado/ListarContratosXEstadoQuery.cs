using Joseco.DDD.Core.Results;
using MediatR;
using SuscripcionesYContratos.Aplicacion.Contratos.DTO;

namespace SuscripcionesYContratos.Aplicacion.Contratos.ListarContratosXEstado;

public sealed record ListarContratosXEstadoQuery(int estado) : IRequest<Result<IReadOnlyList<ContratoDto>>>;