using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;

public sealed class GetTriajeByIdInteractor : IGetTriajeByIdInputPort
{
    private readonly ITriajeRepository _repository;

    public GetTriajeByIdInteractor(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetTriajeByIdOutput> HandleAsync(GetTriajeByIdQuery query, CancellationToken cancellationToken = default)
    {
        var triaje = await _repository.GetByIdAsync(query.Id, cancellationToken)
            ?? throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Triaje,
                "No se encontró el triaje solicitado."
            );

        return new GetTriajeByIdOutput(
            triaje.Id, triaje.Codigo, triaje.FechaHora, triaje.VacunoId,
            triaje.VacunoNombre, triaje.TipoPesoCode, triaje.PesoKg,
            triaje.Observaciones, triaje.EstadoRegistroCode,
            triaje.EncargadoUsuarioId, triaje.CreatedAt);
    }
}
