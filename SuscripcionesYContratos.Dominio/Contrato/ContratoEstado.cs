using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Dominio.Contrato
{
    public enum ContratoEstado
    {
        Activo = 0,
        Cancelado = 1,
        Expirado = 2,
        Finalizado = 3
    }
}
