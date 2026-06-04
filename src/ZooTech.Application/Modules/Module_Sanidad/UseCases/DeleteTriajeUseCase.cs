using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class DeleteTriajeUseCase
{
    private readonly ITriajeRepository _repository;

    public DeleteTriajeUseCase(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExecuteAsync(long id)
    {
        var triaje = await _repository.GetByIdAsync(id);

        if (triaje is null)
            return false;

        await _repository.DeleteAsync(id);
        return true;
    }
}