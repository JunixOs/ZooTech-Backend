using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Models;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;

public sealed class DeleteOrdenioInteractor : IDeleteOrdenioInputPort
{
    private readonly IOrdenioRepository _repository;

    public DeleteOrdenioInteractor(IOrdenioRepository repository)
    {
        _repository = repository;
    }

    public async Task<EmptyOutput> Handle(DeleteOrdenioCommand command, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Produccion_Leche,
                "No se encontró el ordeño solicitado."
            );

        try
        {
            existing.SoftDelete(command.MotivoEliminacion, null, DateTime.UtcNow);
        }
        catch (ArgumentException ex)
        {
            throw new ConflictException(ex.Message);
        }

        _ = await _repository.UpdateAsync(existing, cancellationToken);

        return EmptyOutput.Value;
    }
}
