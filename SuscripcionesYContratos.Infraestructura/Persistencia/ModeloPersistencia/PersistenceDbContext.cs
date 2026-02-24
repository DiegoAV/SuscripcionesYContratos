using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using SuscripcionesYContratos.Dominio.Contrato;
using SuscripcionesYContratos.Dominio.Entregas;
using SuscripcionesYContratos.Dominio.Suscripcion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Infraestructura.Persistencia.ModeloPersistencia
{
    internal class PersistenceDbContext : DbContext, IDatabase
    {
        public DbSet<Suscripciones> Suscripciones { get; set; }
        public DbSet<Contratos> Contratos { get; set; }
        public DbSet<CalendarioEntrega> Entregas { get; set; }

        public Func<QueryContext, TResult> CompileQuery<TResult>(Expression query, bool async)
        {
            throw new NotImplementedException();
        }

        public Expression<Func<QueryContext, TResult>> CompileQueryExpression<TResult>(Expression query, bool async, IReadOnlySet<string> nonNullableReferenceTypeParameters)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges(IList<IUpdateEntry> entries)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync(IList<IUpdateEntry> entries, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
