using Joseco.DDD.Core.Abstractions;
using Joseco.DDD.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Dominio.Contrato
{
    public class Contratos : AggregateRoot
    {
        public Guid pacienteId { get; private set; }
        public Guid suscripcionId { get; private set; }
        public Guid planId { get; private set; }
        public TimeOnly hora { get; private set; }
        public DateOnly inicio { get; private set; }
        public DateOnly fin { get; private set; }
        public ContratoEstado estado { get; private set; }
        public int politicaCancelacionDias { get; private set; }
        public DateTime? updateAt { get; private set; }

        public Contratos()
        {
        }
        public Contratos( Guid pacienteId, Guid suscripcionId, Guid planId, TimeOnly hora, DateOnly inicio, DateOnly fin, int politicaCancelacionDias)
        {
            this.pacienteId = pacienteId;
            this.suscripcionId = suscripcionId;
            this.planId = planId;
            this.hora = hora;
            this.inicio = inicio;
            this.fin = fin;
            this.estado = ContratoEstado.Activo;
            this.politicaCancelacionDias = politicaCancelacionDias;
        }

        public void SetPoliticaCancelacionDias(int dias)
        {
            if (dias <= 0)
                throw new DomainException(ContratoError.CantidadDiasPoliticaInvalida);
            
            this.politicaCancelacionDias = dias;    
            this.updateAt = DateTime.UtcNow;
        }

        public void SetHora(TimeOnly hora)
        {
            if(hora < new TimeOnly(6,30) || hora > new TimeOnly(9,0))
                throw new DomainException(ContratoError.HoraNoValida);
            
            this.hora = hora;
            this.updateAt = DateTime.UtcNow;
        }

        public void cancelarContrato()
        {
            if(this.estado == ContratoEstado.Cancelado)
                throw new DomainException(ContratoError.ContratoYaCancelado);

            this.estado = ContratoEstado.Cancelado;
            this.updateAt = DateTime.UtcNow;
        }
    }
}
