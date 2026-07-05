using FluentValidation;
using System;
using System.Linq;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;

public sealed class DeleteVacunoInteractor : IDeleteVacunoInputPort
{
    private readonly IVacunoRepository _repository;
    private readonly ICeloRepository _celoRepository;
    private readonly ITriajeRepository _triajeRepository;
    private readonly IOrdenioRepository _ordenioRepository;
    private readonly IValidator<DeleteVacunoCommand> _validator;

    public DeleteVacunoInteractor(
        IVacunoRepository repository,
        ICeloRepository celoRepository,
        ITriajeRepository triajeRepository,
        IOrdenioRepository ordenioRepository,
        IValidator<DeleteVacunoCommand> validator)
    {
        _repository = repository;
        _celoRepository = celoRepository;
        _triajeRepository = triajeRepository;
        _ordenioRepository = ordenioRepository;
        _validator = validator;
    }

    public async Task HandleAsync(long id, DeleteVacunoCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var existing = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new VacunoNotFoundException($"No existe el vacuno con el ID {id}.");

        // Validar dependencias (PDF pág 10: "Solo se elimina si no tiene registros dependientes activos: triajes, incidentes, ordeños, etc.")
        if (await _celoRepository.ExistsVacunoAsync(id, cancellationToken))
        {
            throw new VacunoHasDependenciesException("El vacuno tiene registros de celo activos.");
        }

        if (await _ordenioRepository.ExistsVacunoAsync(id, cancellationToken))
        {
            throw new VacunoHasDependenciesException("El vacuno tiene registros de ordeño activos.");
        }

        var triajes = await _triajeRepository.GetHistorialByVacunoIdAsync(id, cancellationToken);
        if (triajes != null && triajes.Any())
        {
            throw new VacunoHasDependenciesException("El vacuno tiene registros de triaje activos.");
        }

        try
        {
            existing.SoftDelete(command.MotivoEliminacion, null, DateTime.UtcNow);
        }
        catch (ArgumentException ex)
        {
            throw new VacunoException(ex.Message, "BAD_REQUEST", 400);
        }

        _ = await _repository.UpdateAsync(existing, null, null, cancellationToken);
    }
}
