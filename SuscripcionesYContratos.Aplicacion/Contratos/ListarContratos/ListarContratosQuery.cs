using Joseco.DDD.Core.Results;
using MediatR;
using SuscripcionesYContratos.Aplicacion.Contratos.DTO;

namespace SuscripcionesYContratos.Aplicacion.Contratos.ListarContratos;

public sealed record ListarContratosQuery() : IRequest<Result<IReadOnlyList<ContratoDto>>>;