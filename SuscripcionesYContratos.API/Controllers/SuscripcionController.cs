using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuscripcionesYContratos.Aplicacion.Suscripciones.CrearSuscripcion;

namespace SuscripcionesYContratos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuscripcionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SuscripcionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CrearSuscripcion([FromBody] CrearSuscripcionCommand command)
        {
            var result = await _mediator.Send(command);

            //return Task.FromResult<IActionResult>(Ok("Crear Suscripcion"));
            return Ok(result);
        }
    }
}
