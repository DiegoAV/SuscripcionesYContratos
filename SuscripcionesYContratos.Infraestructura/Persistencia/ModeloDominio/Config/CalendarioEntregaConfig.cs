using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuscripcionesYContratos.Dominio.Entregas;

namespace SuscripcionesYContratos.Infraestructura.Persistencia.ModeloDominio.Config
{
    public class CalendarioEntregaConfig : IEntityTypeConfiguration<CalendarioEntrega>
    {
        public void Configure(EntityTypeBuilder<CalendarioEntrega> builder)
        {
            // Tabla real en BD (según migraciones): "Entregas"
            builder.ToTable("Entregas");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.contratoId).IsRequired();
            builder.Property(x => x.hora).IsRequired();
            builder.Property(x => x.fecha).IsRequired();

            // Enum -> int (evita integer vs text)
            builder.Property(x => x.estado).HasConversion<int>().IsRequired();

            builder.Ignore("_domainEvents");
            builder.Ignore(x => x.DomainEvents);
        }
    }
}