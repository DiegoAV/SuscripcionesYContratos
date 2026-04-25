using Joseco.DDD.Core.Abstractions;
using Joseco.DDD.Core.Results;
using SuscripcionesYContratos.Dominio.Entregas.Eventos;
using System;

namespace SuscripcionesYContratos.Dominio.Entregas
{
    public class CalendarioEntrega : AggregateRoot
    {
        public Guid contratoId { get; private set; }
        public DateOnly fecha { get; private set; }
        public TimeOnly hora { get; private set; }
        public CalendarioEntregaEstado estado { get; private set; }
        public DateTime? updateAt { get; private set; }

        public CalendarioEntrega()
        {
        }

        public CalendarioEntrega(Guid contratoId, DateOnly fecha, TimeOnly hora)
        {
            this.contratoId = contratoId;
            this.fecha = fecha;
            this.hora = hora;
            this.estado = CalendarioEntregaEstado.Programado;
            AddStatusChangedDomainEvent();
        }

        public void ReprogramarEntrega(DateOnly nuevaFecha, TimeOnly nuevaHora)
        {
            if (this.estado == CalendarioEntregaEstado.Entregado)
                throw new DomainException(CalendarioEntregaError.CalendarioEntregaYaEntregado);
            if (this.estado == CalendarioEntregaEstado.Cancelado)
                throw new DomainException(CalendarioEntregaError.CalendarioEntregaYaCancelado);

            // Permitir múltiples cambios cuando solo se modifica la hora.
            // Solo bloquea si ya estaba reprogramado y además se intenta cambiar la fecha.
            if (this.estado == CalendarioEntregaEstado.Reprogramado && nuevaFecha != this.fecha)
                throw new DomainException(CalendarioEntregaError.CalendarioEnrtegaYaReprogramado);

            this.fecha = nuevaFecha;
            this.hora = nuevaHora;
            this.estado = CalendarioEntregaEstado.Reprogramado;
            this.updateAt = DateTime.UtcNow;
        }

        public void Cancelar()
        {
            if (this.estado == CalendarioEntregaEstado.Entregado)
                throw new DomainException(CalendarioEntregaError.CalendarioEntregaYaEntregado);

            if (this.estado == CalendarioEntregaEstado.Cancelado)
                throw new DomainException(CalendarioEntregaError.CalendarioEntregaYaCancelado);

            this.estado = CalendarioEntregaEstado.Cancelado;
            this.updateAt = DateTime.UtcNow;
        }

        private void AddStatusChangedDomainEvent()
        {
            AddDomainEvent(new CalendarioEntregaDomainEvent(contratoId, contratoId, fecha, hora, (int)estado, DateTime.UtcNow));
        }
    }
}
