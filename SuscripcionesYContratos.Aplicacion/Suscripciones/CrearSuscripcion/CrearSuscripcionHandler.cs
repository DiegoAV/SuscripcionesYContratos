using Joseco.DDD.Core.Abstractions;
using Joseco.DDD.Core.Results;
using MediatR;
using SuscripcionesYContratos.Dominio.Suscripcion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuscripcionesYContratos.Aplicacion.Suscripciones.CrearSuscripcion
{
    public class CrearSuscripcionHandler : IRequestHandler<CrearSuscripcionCommand, Result<Guid>>
    {
        private readonly ISuscripcionesRepo _suscripcionesRepo;
        private readonly IUnitOfWork _unitOfWork;
        public CrearSuscripcionHandler(ISuscripcionesRepo suscripcionesRepo, IUnitOfWork unitOfWork)
        {
            _suscripcionesRepo = suscripcionesRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(CrearSuscripcionCommand request, CancellationToken cancellationToken)
        {
            Guid guid = Guid.NewGuid();
            var suscripcion = new Dominio.Suscripcion.Suscripciones(guid, request.nombre, request.descripcion, request.cantidadDias, request.precioDia);
            //  var susb = new Suscripciones(request.suscripcionID, request.nombre, request.descripcion, request.cantidadEntregas, request.precio, request.incluyeFinDeSemana);

            await _suscripcionesRepo.AddAsync(suscripcion);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Success(suscripcion.Id);
        }
    }
}
