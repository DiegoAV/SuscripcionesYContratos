using Joseco.DDD.Core.Abstractions;
using Joseco.DDD.Core.Results;
using SuscripcionesYContratos.Dominio.Contrato;
using SuscripcionesYContratos.Dominio.Entregas;
using SuscripcionesYContratos.Dominio.Suscripcion.Eventos;
using System;

namespace SuscripcionesYContratos.Dominio.Suscripcion
{
    public class Suscripciones : AggregateRoot
    {
        public string nombre { get; private set; }
        public string descripcion { get; private set; }

        public int cantidadDias { get; private set; }

        public decimal precioDia { get; private set; } // <-- antes precio

        public DateTime? updateAt { get; private set; }

        private Suscripciones() { }

        public Suscripciones(
            Guid suscripcionID,
            string nombre,
            string descripcion,
            int cantidadDias,
            decimal precioDia) : base(suscripcionID)
        {
            this.nombre = nombre;
            this.descripcion = descripcion;
            this.cantidadDias = cantidadDias;
            this.precioDia = precioDia;
            AddStatusChangedDomainEvent();

        }

        public void SetNombre(string nuevoNombre)
        {
            if (string.IsNullOrWhiteSpace(nuevoNombre))
                throw new DomainException(SuscripcionError.NombreInvalido);

            nombre = nuevoNombre.Trim();
            updateAt = DateTime.UtcNow;
        }

        public void SetDescripcion(string nuevaDescripcion)
        {
            if (string.IsNullOrWhiteSpace(nuevaDescripcion))
                throw new DomainException(SuscripcionError.DescripcionInvalida);

            descripcion = nuevaDescripcion.Trim();
            updateAt = DateTime.UtcNow;
        }

        public void SetPrecioDia(decimal nuevoPrecioDia)
        {
            if (nuevoPrecioDia < 0)
                throw new DomainException(SuscripcionError.PrecioInvalido);

            this.precioDia = nuevoPrecioDia;
            this.updateAt = DateTime.UtcNow;
        }
        public void SetCantidadDias(int nuevosDias)
        {
            if (nuevosDias <= 0)
                throw new DomainException(SuscripcionError.CantidadDiasInvalida);

            cantidadDias = nuevosDias;
            updateAt = DateTime.UtcNow;
        }


        private void AddStatusChangedDomainEvent()
        {
            AddDomainEvent(new SuscripcionChangeDomainEvent(Id, nombre, descripcion, cantidadDias, precioDia, 0, DateTime.UtcNow));
            //AddDomainEvent(new SuscripcionChangeDomainEvent(suscripcionId, nombre, descripcion, cantidadDias, precioDia, (int)estado, DateTime.UtcNow));
        }
    }
}
