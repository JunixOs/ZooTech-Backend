using System;
using System.Threading;
using System.Threading.Tasks;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Domain.Module_Fecundacion.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

public sealed class DeleteFecundacionInteractor : IDeleteFecundacionInputPort
{
    private readonly IFecundacionRepository _repository;

    public DeleteFecundacionInteractor(IFecundacionRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(long id, DeleteFecundacionCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Razon))
        {
            throw new ArgumentException("Falta razón de eliminación o datos inválidos.");
        }

        var existing = await _repository.GetForEditAsync(id, cancellationToken);
        if (existing is null)
        {
            throw new FecundacionNotFoundException();
        }

        // Validar si tiene crías vinculadas en trazabilidad
        var hasCria = await _repository.HasCriaAsync(id, cancellationToken);
        if (hasCria)
        {
            throw new FecundacionHasDependenciesException();
        }

        await _repository.DeleteAsync(id, command.Razon, cancellationToken);
    }
}
