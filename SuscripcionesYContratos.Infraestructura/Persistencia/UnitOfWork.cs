using Joseco.DDD.Core.Abstractions;
using SuscripcionesYContratos.Dominio.Entregas;
using SuscripcionesYContratos.Infraestructura.Outbox;
using SuscripcionesYContratos.Infraestructura.Persistencia.ModeloDominio;
using System.Collections.Immutable;
using System.Text.Json;

namespace SuscripcionesYContratos.Infraestructura.Persistencia
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly DomainDbContext _dbContext;
        private const string CalendarioEntregaCreadaEventName = "calendarioentrega.creada";

        public UnitOfWork(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            var domainEvents = _dbContext.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents.Any())
                .Select(x =>
                {
                    var domainEvents = x.Entity.DomainEvents.ToImmutableArray();
                    x.Entity.ClearDomainEvents();

                    return domainEvents;
                })
                .SelectMany(domainEvents => domainEvents)
                .ToList();

            var outboxMessages = domainEvents
                .Select(MapToOutboxMessage)
                .Where(message => message is not null)
                .Cast<OutboxMessage>()
                .ToList();

            if (outboxMessages.Count > 0)
            {
                await _dbContext.OutboxMessages.AddRangeAsync(outboxMessages, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private static DateTime EnsureUtc(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };
        }

        private OutboxMessage? MapToOutboxMessage(DomainEvent domainEvent)
        {
            if (domainEvent is CalendarioEntregaDomainEvent packageEvent)
            {
                var payload = new
                {
                    entregaId = packageEvent.entregaId,
                    contratoId = packageEvent.contratoId,
                    fecha = packageEvent.fecha,
                    hora = packageEvent.hora,
                    estado = packageEvent.estado,
                    occurredOn = packageEvent.occurredOnUtc
                };

                var occurredOnUtc = EnsureUtc(packageEvent.occurredOnUtc);

                return new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    EventName = CalendarioEntregaCreadaEventName,
                    Type = packageEvent.GetType().AssemblyQualifiedName ?? nameof(CalendarioEntregaDomainEvent),
                    Payload = JsonSerializer.Serialize(payload),
                    OccurredOnUtc = occurredOnUtc
                };
            }

            return null;
        }
    }
}
