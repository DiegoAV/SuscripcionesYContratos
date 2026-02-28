using MediatR;
using Microsoft.AspNetCore.Mvc;
using SuscripcionesYContratos.Aplicacion.Contratos.CrearContrato;
using SuscripcionesYContratos.Aplicacion.Contratos.ListarContratos;
using SuscripcionesYContratos.Aplicacion.Contratos.ListarContratosXEstado;
using SuscripcionesYContratos.Aplicacion.Contratos.ListarContratosXPaciente;

namespace SuscripcionesYContratos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContratoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContratoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CrearContrato([FromBody] CrearContratoCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> ListarContratos(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ListarContratosQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("paciente/{pacienteId:guid}")]
        public async Task<IActionResult> ListarContratosXPaciente([FromRoute] Guid pacienteId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ListarContratosXPacienteQuery(pacienteId), cancellationToken);
            return Ok(result);
        }

        [HttpGet("estado/{estado:int}")]
        public async Task<IActionResult> ListarContratosXEstado([FromRoute] int estado, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ListarContratosXEstadoQuery(estado), cancellationToken);
            return Ok(result);
        }
    }
}