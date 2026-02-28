using Joseco.DDD.Core.Abstractions;
using Joseco.DDD.Core.Results;
using MediatR;
using SuscripcionesYContratos.Dominio.Suscripcion;

namespace SuscripcionesYContratos.Aplicacion.Suscripciones.ActualizarSuscripcion;

internal sealed class ActualizarSuscripcionHandler : IRequestHandler<ActualizarSuscripcionCommand, Result<Guid>>
{
    private readonly ISuscripcionesRepo _repo;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarSuscripcionHandler(ISuscripcionesRepo repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ActualizarSuscripcionCommand request, CancellationToken cancellationToken)
    {
        var suscripcion = await _repo.GetByIdAsync(request.suscripcionId, readOnly: false);

        if (suscripcion is null)
            return Result.Failure<Guid>(SuscripcionError.SuscripcionNoEncontrada);

        var hayNombre = request.nombre is not null;
        var hayDescripcion = request.descripcion is not null;
        var hayCantidadDias = request.cantidadDias.HasValue;
        var hayPrecioDia = request.precioDia.HasValue;

        if (!hayNombre && !hayDescripcion && !hayCantidadDias && !hayPrecioDia)
            return Result.Failure<Guid>(SuscripcionError.ActualizacionSinCambios);

        if (hayNombre)
            suscripcion.SetNombre(request.nombre!);

        if (hayDescripcion)
            suscripcion.SetDescripcion(request.descripcion!);

        if (hayCantidadDias)
            suscripcion.SetCantidadDias(request.cantidadDias!.Value);

        if (hayPrecioDia)
            suscripcion.SetPrecioDia(request.precioDia!.Value);

        await _repo.UpdateAsync(suscripcion);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(suscripcion.Id);
    }
}