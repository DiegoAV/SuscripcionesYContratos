using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Joseco.DDD.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using SuscripcionesYContratos.Dominio.Contrato;
using SuscripcionesYContratos.Dominio.Entregas;
using SuscripcionesYContratos.Dominio.Suscripcion;
using SuscripcionesYContratos.Infraestructura.Outbox;


namespace SuscripcionesYContratos.Infraestructura.Persistencia.ModeloDominio
{
    public class DomainDbContext : DbContext
    {
        public DbSet<Suscripciones> Suscripciones { get; set; }
        public DbSet<Contratos> Contratos { get; set; }
        public DbSet<CalendarioEntrega> Entregas { get; set; }
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        public DomainDbContext(DbContextOptions<DomainDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
            modelBuilder.Entity<OutboxMessage>(b =>
            {
                b.ToTable("OutboxMessages");
                b.HasKey(x => x.Id);
                b.Property(x => x.Type).HasMaxLength(400).IsRequired();
                b.Property(x => x.Payload).IsRequired();
                b.Property(x => x.OccurredOnUtc).IsRequired();
                b.HasIndex(x => x.ProcessedOnUtc);
            });

            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<DomainEvent>();
        }
    }
}
