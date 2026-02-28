using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuscripcionesYContratos.Dominio.Contrato;

namespace SuscripcionesYContratos.Infraestructura.Persistencia.ModeloDominio.Config
{
    public class ContratoConfig : IEntityTypeConfiguration<Contratos>
    {
        public void Configure(EntityTypeBuilder<Contratos> builder)
        {
            builder.ToTable("Contratos");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.pacienteId).IsRequired();
            builder.Property(x => x.suscripcionId).IsRequired();
            builder.Property(x => x.planId).IsRequired();
            builder.Property(x => x.hora).IsRequired();
            builder.Property(x => x.inicio).IsRequired();
            builder.Property(x => x.fin).IsRequired();

            builder.Property(x => x.incluyeFinDeSemana).IsRequired();
            builder.Property(x => x.cantidadEntregas).IsRequired();

            builder.Property(x => x.precioTotal).IsRequired(); // <-- NUEVO

            // Guardar enum como integer (compatible con la BD actual)
            builder.Property(x => x.estado).HasConversion<int>().IsRequired();

            builder.Property(x => x.politicaCancelacionDias).IsRequired();

            builder.Ignore("_domainEvents");
            builder.Ignore(x => x.DomainEvents);
        }
    }
}
