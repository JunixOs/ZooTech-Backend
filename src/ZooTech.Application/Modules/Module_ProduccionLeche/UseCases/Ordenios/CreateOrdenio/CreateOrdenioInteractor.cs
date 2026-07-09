using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Domain.Module_ProduccionLeche.Entities;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;

public sealed class CreateOrdenioInteractor : ICreateOrdenioInputPort
{
    private readonly IOrdenioUnitOfWork _unitOfWork;

    public CreateOrdenioInteractor(IOrdenioUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateOrdenioOutput> Handle(
        CreateOrdenioCommand command, 
        CancellationToken cancellationToken
    )
    {
        var repository = _unitOfWork.Repository;

        await OrdenioReferenceValidator.EnsureReferencesExistAsync(
            repository,
            command.VacunoId,
            command.EncargadoUsuarioId,
            command.EstadoOrdenioCode,
            cancellationToken);

        if (await repository.ExistsCodigoAsync(command.Codigo, cancellationToken))
        {
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Produccion_Leche,
                new List<string>
                {
                    "ORDENIO-ORDENIO-CODE-ALREADY_EXISTS"
                }
            );
        }

        if (await repository.ExistsVacunoFechaAsync(command.VacunoId, command.FechaHora, null, cancellationToken))
        {
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Produccion_Leche,
                new List<string>
                {
                    "ORDENIO-ORDENIO-DATE-ALREADY_EXISTS"
                }
            );
        }

        Ordenio ordenio;
        try
        {
            ordenio = Ordenio.CreateNew(
                command.Codigo,
                command.FechaHora,
                command.VacunoId,
                command.EncargadoUsuarioId,
                command.Litros,
                command.EstadoOrdenioCode,
                command.Observaciones,
                null,
                DateTime.UtcNow);
        }
        catch (ArgumentException ex)
        {
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Produccion_Leche,
                message: ex.Message
            );
        }

        var saved = await _unitOfWork.ExecuteInTransactionAsync(
            ct => repository.AddAsync(ordenio, ct),
            cancellationToken,
            async (_, ct) => await repository.GetByCodigoAsync(command.Codigo, ct)
                ?? throw new InvalidOperationException("No se pudo recuperar el ordeño creado."));
        return new CreateOrdenioOutput(OrdenioMapper.ToOutput(saved));
    }
}
