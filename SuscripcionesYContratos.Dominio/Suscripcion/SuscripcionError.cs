using Joseco.DDD.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Dominio.Suscripcion
{
    public static class SuscripcionError
    {
        public static readonly Error SuscripcionNoEncontrada =
                new(
                    "Suscripcion.NoEncontrada",
                    "La suscripción solicitada no fue encontrada.",
                    ErrorType.NotFound);

        public static readonly Error SuscripcionInvalida =
            new(
                "Suscripcion.Invalida",
                "La suscripción proporcionada es inválida.",
                ErrorType.Validation);

        public static readonly Error PrecioInvalido =
            new(
                "Suscripcion.PrecioInvalido",
                "El precio de la suscripción no puede ser negativo.",
                ErrorType.Validation);

        public static readonly Error CantidadEntregasInvalida =
                new(
                    "Suscripcion.CantidadEntregasInvalida",
                    "La cantidad de entregas de la suscripción debe ser mayor a cero.",
                    ErrorType.Validation);
    }
}
