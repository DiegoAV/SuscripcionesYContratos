using Joseco.DDD.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Dominio.Contrato
{
    public static class ContratoError
    {
        public static readonly Error ContratoNoEncontrado =
            new(
                "Contrato.NoEncontrado",
                "El contrato solicitado no fue encontrado.",
                ErrorType.NotFound);

        public static readonly Error ContratoInvalido =
            new(
                "Contrato.Invalido",
                "El contrato proporcionado es inválido.",
                ErrorType.Validation);

        public static readonly Error CantidadDiasPoliticaInvalida =
            new(
                "Contrato.CantidadDiasPoliticaInvalida",
                "La cantidad de días de la política no puede ser menor o igual a cero.",
                ErrorType.Validation);

        public static readonly Error ContratoYaCancelado =
            new(
                "Contrato.YaCancelado",
                "El contrato ya se encuentra cancelado.",
                ErrorType.Conflict);

        public static readonly Error ContratoYaExpirado =
            new(
                "Contrato.YaExpirado",
                "El contrato ya se encuentra expirado.",
                ErrorType.Conflict);

        public static readonly Error HoraNoValida =
            new(
                "Contrato.HoraNoValida",
                "La hora proporcionada no es válida, los horarios de entrega son entre 06:30 AM a 09:00 AM.",
                ErrorType.Validation);
    }
}
