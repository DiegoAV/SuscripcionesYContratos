using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuscripcionesYContratos.Aplicacion.CalendarioEntrega.CrearCalendarioEntrega;
using SuscripcionesYContratos.Aplicacion.Contratos.CrearContrato;

namespace SuscripcionesYContratos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarioEntregaController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CalendarioEntregaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CrearCalendarioEntrega([FromBody] CrearCalendarioEntregaCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
