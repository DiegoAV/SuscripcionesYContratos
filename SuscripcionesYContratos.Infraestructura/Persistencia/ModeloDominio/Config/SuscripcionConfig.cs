using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuscripcionesYContratos.Dominio.Suscripcion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Infraestructura.Persistencia.ModeloDominio.Config
{
    public class SuscripcionConfig : IEntityTypeConfiguration<Suscripciones>
    {
        public void Configure(EntityTypeBuilder<Suscripciones> builder)
        {
            builder.ToTable("Suscripciones");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.nombre).HasMaxLength(200).IsRequired();
            builder.Property(x => x.descripcion).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.cantidadDias).IsRequired();
            builder.Property(x => x.precioDia).HasColumnType("decimal(18,2)").IsRequired();

            builder.Ignore("_domainEvents");
            builder.Ignore(x => x.DomainEvents);
        }
    }
}
