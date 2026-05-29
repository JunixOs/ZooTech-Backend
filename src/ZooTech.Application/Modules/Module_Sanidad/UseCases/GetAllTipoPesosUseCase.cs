using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class GetAllTipoPesosUseCase
{
    private readonly ITriajeRepository _repository;

    public GetAllTipoPesosUseCase(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TipoPesoResponse>> ExecuteAsync()
    {
        var tipos = await _repository.GetAllTipoPesosAsync();

        return tipos.Select(t => new TipoPesoResponse
        {
            Code = t.Code,
            Nombre = t.Nombre
        });
    }
}