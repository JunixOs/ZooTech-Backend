using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;

public sealed class GetOrdenioByIdInteractor : IGetOrdenioByIdInputPort
{
    private readonly IOrdenioRepository _repository;

    public GetOrdenioByIdInteractor(IOrdenioRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetOrdenioByIdOutput> HandleAsync(long id, CancellationToken cancellationToken)
    {
        var ordenio = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ORDENIO_NOT_FOUND", $"No se encontró el ordeño solicitado {id}.");

        return new GetOrdenioByIdOutput(OrdenioMapper.ToOutput(ordenio));
    }
}
