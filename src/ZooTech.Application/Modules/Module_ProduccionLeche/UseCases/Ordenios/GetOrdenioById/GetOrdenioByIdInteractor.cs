using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;

public sealed class GetOrdenioByIdInteractor : IGetOrdenioByIdInputPort
{
    private readonly IOrdenioRepository _repository;

    public GetOrdenioByIdInteractor(IOrdenioRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetOrdenioByIdOutput> HandleAsync(GetOrdenioByIdQuery query, CancellationToken cancellationToken)
    {
        var ordenio = await _repository.GetByIdAsync(query.Id, cancellationToken)
            ?? throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Produccion_Leche,
                "No se encontró el ordeño solicitado."
            );

        return new GetOrdenioByIdOutput(OrdenioMapper.ToOutput(ordenio));
    }
}
