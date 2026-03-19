using Joseco.DDD.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Dominio.Entregas
{
    public static class CalendarioEntregaError
    {
        public static readonly Error CalendarioEntregaNoEncontrado =
            new(
                "CalendarioEntrega.NoEncontrado",
                "El calendario de entrega solicitado no fue encontrado.",
                ErrorType.NotFound);

        public static readonly Error CalendarioEntregaInvalido =
            new(
                "CalendarioEntrega.Invalido",
                "El calendario de entrega proporcionado es inválido.",
                ErrorType.Validation);

        public static readonly Error CalendarioEnrtegaYaReprogramado =
            new(
                "CalendarioEntrega.YaReprogramado",
                "El calendario de entrega ya se encuentra reprogramado, solo una vez se puede reprogramar una entrega.",
                ErrorType.Conflict);

        public static readonly Error CalendarioEntregaYaEntregado =
            new(
                "CalendarioEntrega.YaEntregado",
                "El calendario de entrega ya se encuentra entregado.",
                ErrorType.Conflict);

         public static readonly Error CalendarioEntregaYaCancelado =
            new(
                "CalendarioEntrega.YaCancelado",
                "El calendario de entrega ya se encuentra cancelado.",
                ErrorType.Conflict);

        public static readonly Error ContratoNoExistente =
            new(
                "CalendarioEntrega.ContratoNoExistente",
                "El Contrato No Existe.",
                ErrorType.Conflict);
    }
}
