using Joseco.DDD.Core.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Aplicacion.Suscripciones.CrearSuscripcion
{
    public record CrearSuscripcionCommand(string nombre, string descripcion, int cantidadEntregas, decimal precio, bool incluyeFinDeSemana) : IRequest<Result<Guid>>;
}
