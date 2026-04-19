using Joseco.DDD.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using SuscripcionesYContratos.Dominio.Contrato;
using SuscripcionesYContratos.Dominio.Entregas;
using SuscripcionesYContratos.Dominio.Suscripcion;
using SuscripcionesYContratos.Infraestructura.Outbox;
using SuscripcionesYContratos.Infraestructura.Persistencia.ModeloPersistencia.EFCoreEntities;

namespace SuscripcionesYContratos.Infraestructura.Persistencia.ModeloPersistencia
{
    public class PersistenceDbContext : DbContext, IDatabase
    {
        public PersistenceDbContext(DbContextOptions<PersistenceDbContext> options) : base(options)
        {
        }

        public DbSet<SuscripcionesPersistenceModel> Suscripciones { get; set; }
        public DbSet<ContratosPersistenceModel> Contratos { get; set; }
        public DbSet<CalendarioEntregaPersistenceModel> Entregas { get; set; }
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<DomainEvent>();

            modelBuilder.Entity<OutboxMessage>(b =>
            {
                b.ToTable("OutboxMessages");
                b.HasKey(x => x.Id);
                b.Property(x => x.EventName).HasColumnName("eventname").IsRequired();
                b.Property(x => x.Type).HasMaxLength(400).IsRequired();
                b.Property(x => x.Payload).IsRequired();
                b.Property(x => x.OccurredOnUtc).IsRequired();
                b.HasIndex(x => x.ProcessedOnUtc);
            });

            base.OnModelCreating(modelBuilder);
        }

        public void Migrate()
        {
            Database.Migrate();
        }
    }
}
