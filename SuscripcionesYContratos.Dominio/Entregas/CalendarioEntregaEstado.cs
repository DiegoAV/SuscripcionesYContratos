using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Dominio.Entregas
{
    public enum CalendarioEntregaEstado
    {
        Programado = 0,
        Reprogramado = 1,
        Entregado = 2,
        Cancelado = 3
    }
}
