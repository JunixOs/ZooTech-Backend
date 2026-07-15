using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

public sealed class DeleteFecundacionInteractor : IDeleteFecundacionInputPort
{
    private readonly IFecundacionRepository _repository;

    public DeleteFecundacionInteractor(IFecundacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<EmptyOutput> HandleAsync(DeleteFecundacionCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Razon))
        {
            throw new ArgumentException("Falta razón de eliminación o datos inválidos.");
        }

        var existing = await _repository.GetForEditAsync(command.Id, cancellationToken);
        if (existing is null)
        {
            throw new FecundacionNotFoundException();
        }

        // Validar si tiene crías vinculadas en trazabilidad
        var hasCria = await _repository.HasCriaAsync(command.Id, cancellationToken);
        if (hasCria)
        {
            throw new FecundacionHasDependenciesException();
        }

        await _repository.DeleteAsync(command.Id, command.Razon, cancellationToken);

        return EmptyOutput.Value;
    }
}
