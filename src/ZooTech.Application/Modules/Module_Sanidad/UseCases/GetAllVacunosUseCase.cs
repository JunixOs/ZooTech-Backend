using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class GetAllVacunosUseCase
{
    private readonly ITriajeRepository _repository;

    public GetAllVacunosUseCase(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<VacunoOptionResponse>> ExecuteAsync()
    {
        var vacunos = await _repository.GetAllVacunosAsync();

        return vacunos.Select(v => new VacunoOptionResponse
        {
            Id = v.Id,
            Codigo = v.Codigo,
            Nombre = v.Nombre
        });
    }
}