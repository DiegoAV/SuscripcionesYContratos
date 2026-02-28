using Joseco.DDD.Core.Results;
using MediatR;
using SuscripcionesYContratos.Aplicacion.Contratos.DTO;

namespace SuscripcionesYContratos.Aplicacion.Contratos.ListarContratosXPaciente;

public sealed record ListarContratosXPacienteQuery(Guid pacienteId) : IRequest<Result<IReadOnlyList<ContratoDto>>>;