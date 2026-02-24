using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;


namespace SuscripcionesYContratos.Core.Abstracciones
{
    public abstract record DomainEvent : INotification
    {
        public Guid Id { get; set; }
        public DateTime OccurredOn { get; set; }

        public DomainEvent()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.Now;
        }
    }
}
