using ZooTech.Application.Common.Models;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;

public sealed class GetFecundacionOptionsInteractor : IGetFecundacionOptionsInputPort
{
    private readonly IFecundacionRepository _repository;

    public GetFecundacionOptionsInteractor(IFecundacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetFecundacionOptionsOutput> HandleAsync(
        EmptyCommand emptyCommand,
        CancellationToken cancellationToken = default
    )
    {
        var data = await _repository.GetOptionsAsync(cancellationToken);

        return new GetFecundacionOptionsOutput(
            data.Tipos.Select(ToOutput).ToList(),
            data.Resultados.Select(ToOutput).ToList(),
            data.Estados.Select(ToOutput).ToList());
    }

    private static FecundacionOptionOutput ToOutput(FecundacionOptionData option)
        => new(option.Code, option.Nombre, option.Descripcion);
}
