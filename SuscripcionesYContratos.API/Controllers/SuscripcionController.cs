using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuscripcionesYContratos.Aplicacion.Suscripciones.ActualizarSuscripcion;
using SuscripcionesYContratos.Aplicacion.Suscripciones.CrearSuscripcion;
using SuscripcionesYContratos.Aplicacion.Suscripciones.ListarSuscripciones;
using SuscripcionesYContratos.Aplicacion.Suscripciones.ObtenerSuscripcion;

namespace SuscripcionesYContratos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOrSuscripcion")]
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
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> ListarSuscripciones(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ListarSuscripcionesQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> ActualizarSuscripcion(
            [FromRoute] Guid id,
            [FromBody] ActualizarSuscripcionBody body,
            CancellationToken cancellationToken)
        {
            var command = new ActualizarSuscripcionCommand(
                id,
                body.nombre,
                body.descripcion,
                body.cantidadDias,
                body.precioDia);

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }

    public sealed record ActualizarSuscripcionBody(
        string? nombre,
        string? descripcion,
        int? cantidadDias,
        decimal? precioDia);
}
