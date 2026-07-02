using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;

public sealed class GetHistorialGeneralInteractor : IGetHistorialGeneralInputPort
{
    private readonly ITriajeRepository _repository;

    public GetHistorialGeneralInteractor(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetHistorialGeneralOutput> HandleAsync(string? desde, string? hasta, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetHistorialGeneralAsync(desde, hasta, cancellationToken);
        return new GetHistorialGeneralOutput(items);
    }
}
