using MediatR;
using Microsoft.AspNetCore.Mvc;
using SuscripcionesYContratos.Aplicacion.CalendarioEntrega.CrearCalendarioEntrega;
using SuscripcionesYContratos.Aplicacion.CalendarioEntrega.EditarCalendarioEntrega;
using SuscripcionesYContratos.Aplicacion.CalendarioEntrega.ListarCalendarioEntrega;
using System;

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

        [HttpGet]
        public async Task<IActionResult> ListarCalendarioEntrega(
            [FromQuery] Guid? contratoId,
            [FromQuery] DateOnly? desde,
            [FromQuery] DateOnly? hasta,
            CancellationToken cancellationToken)
        {
            var query = new ListarCalendarioEntregaQuery(
                contratoId: contratoId,
                desde: desde,
                hasta: hasta);

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{entregaId:guid}")]
        public async Task<IActionResult> EditarCalendarioEntrega(
            [FromRoute] Guid entregaId,
            [FromBody] EditarCalendarioEntregaCommand body,
            CancellationToken cancellationToken)
        {
            // Fuerza el Id desde la ruta para evitar inconsistencias
            var command = body with { entregaId = entregaId };

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
