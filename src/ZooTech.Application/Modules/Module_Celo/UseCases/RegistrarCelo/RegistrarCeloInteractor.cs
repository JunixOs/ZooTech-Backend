using FluentValidation;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.RegistrarCelo;

public sealed class RegistrarCeloInteractor : IRegistrarCeloInputPort
{
    private readonly ICeloRepository _celoRepository;
    private readonly IValidator<RegistrarCeloCommand> _validator;

    public RegistrarCeloInteractor(ICeloRepository celoRepository, IValidator<RegistrarCeloCommand> validator)
    {
        _celoRepository = celoRepository;
        _validator = validator;
    }

    public async Task<RegistrarCeloOutput> HandleAsync(
        RegistrarCeloCommand command,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);
        // Validar que el vacuno existe
        if (!await _celoRepository.ExistsVacunoAsync(command.VacunoId, cancellationToken))
        {
            throw new ConflictException($"El vacuno con ID {command.VacunoId} no existe.");
        }

        // Generar código único
        var codigo = $"CELO-{DateTime.UtcNow:yyyyMMddHHmmss}";
        if (await _celoRepository.ExistsCodigoAsync(codigo, cancellationToken))
        {
            codigo = $"CELO-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

        var utcNow = DateTime.UtcNow;

        var celo = Celo.CreateNew(
            codigo: codigo,
            fechaHora: command.FechaHora,
            vacunoId: command.VacunoId,
            encargadoUsuarioId: command.EncargadoUsuarioId,
            observaciones: command.Observaciones,
            estadoRegistroCode: "ACTIVO",
            caracteristicaCodes: command.CaracteristicaCodes,
            actorUsuarioId: command.EncargadoUsuarioId,
            utcNow: utcNow);

        var saved = await _celoRepository.AddAsync(celo, cancellationToken);

        return new RegistrarCeloOutput(
            Id: saved.Id,
            Codigo: saved.Codigo,
            FechaHora: saved.FechaHora);
    }
}
