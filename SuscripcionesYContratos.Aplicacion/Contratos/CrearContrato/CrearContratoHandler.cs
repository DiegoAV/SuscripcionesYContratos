using Joseco.DDD.Core.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Aplicacion.Contratos.CrearContrato
{
    internal class CrearContratoHandler : IRequestHandler<CrearContratoCommand, Result<Guid>>
    {
        public Task<Result<Guid>> Handle(CrearContratoCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
