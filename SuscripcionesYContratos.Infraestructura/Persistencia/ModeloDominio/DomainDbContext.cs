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


namespace SuscripcionesYContratos.Infraestructura.Persistencia.ModeloDominio
{
    internal class DomainDbContext : DbContext
    {
        public DbSet<Suscripciones> Suscripciones { get; set; }
        public DbSet<Contratos> Contratos { get; set; }
        public DbSet<CalendarioEntrega> Entregas { get; set; }
        public DomainDbContext(DbContextOptions<DomainDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<DomainEvent>();

            //base.OnModelCreating(modelBuilder);
            //// Configuración de la entidad Suscripciones
            //modelBuilder.Entity<Suscripciones>(entity =>
            //{
            //    entity.HasKey(e => e.suscripcionID);
            //    entity.Property(e => e.nombre).IsRequired().HasMaxLength(100);
            //    entity.Property(e => e.descripcion).HasMaxLength(500);
            //    entity.Property(e => e.precio).HasColumnType("decimal(18,2)");
            //});
            //// Configuración de la entidad Contratos
            //modelBuilder.Entity<Contratos>(entity =>
            //{
            //    entity.HasKey(e => e.contratoId);
            //    entity.Property(e => e.hora).HasConversion(
            //        v => v.ToString("HH:mm"),
            //        v => TimeOnly.Parse(v));
            //    entity.Property(e => e.inicio).HasConversion(
            //        v => v.ToString("yyyy-MM-dd"),
            //        v => DateOnly.Parse(v));
            //    entity.Property(e => e.fin).HasConversion(
            //        v => v.ToString("yyyy-MM-dd"),
            //        v => DateOnly.Parse(v));
            //});
            //// Configuración de la entidad CalendarioEntrega
            //modelBuilder.Entity<CalendarioEntrega>(entity =>
            //{
            //    entity.HasKey(e => e.entregaId);
            //    entity.Property(e => e.fecha).HasConversion(
            //        v => v.ToString("yyyy-MM-dd"),
            //        v => DateOnly.Parse(v));
            //});

        }
    }
}
