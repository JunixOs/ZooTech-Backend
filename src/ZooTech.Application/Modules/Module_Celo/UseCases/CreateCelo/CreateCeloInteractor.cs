using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;

public sealed class CreateCeloInteractor : ICreateCeloInputPort
{
    private readonly ICeloRepository _celoRepository;

    public CreateCeloInteractor(ICeloRepository celoRepository)
    {
        _celoRepository = celoRepository;
    }

    public async Task<CreateCeloOutput> Handle(
        CreateCeloCommand command,
        CancellationToken cancellationToken = default
    )
    {
        if (!await _celoRepository.ExistsVacunoAsync(command.VacunoId, cancellationToken))
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Celo,
                new List<string>()
                {
                    "CELO-VACUNO-ID-NOT_EXISTS",
                }
            );

        var codigo = $"CELO-{DateTime.UtcNow:yyyyMMddHHmmss}";
        if (await _celoRepository.ExistsCodigoAsync(codigo, cancellationToken))
            codigo = $"CELO-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        var utcNow = DateTime.UtcNow;

        var celo = Celo.CreateNew(
            codigo: codigo,
            fechaHora: command.FechaHora.GetValueOrDefault(),
            vacunoId: command.VacunoId,
            encargadoUsuarioId: command.EncargadoUsuarioId,
            observaciones: command.Observaciones,
            estadoRegistroCode: "ACTIVO",
            caracteristicaCodes: command.CaracteristicaCodes,
            actorUsuarioId: command.EncargadoUsuarioId,
            utcNow: utcNow);

        var saved = await _celoRepository.AddAsync(celo, cancellationToken);

        return new CreateCeloOutput(
            Id: saved.Id,
            Codigo: saved.Codigo,
            FechaHora: saved.FechaHora);
    }
}
