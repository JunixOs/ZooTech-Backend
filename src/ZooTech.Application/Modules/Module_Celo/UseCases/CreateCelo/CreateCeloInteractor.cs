using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;

public sealed class CreateCeloInteractor : ICreateCeloInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;

    public CreateCeloInteractor(IGanaderiaUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCeloOutput> Handle(
        CreateCeloCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var repository = _unitOfWork.Celos;

        if (!await repository.ExistsVacunoAsync(command.VacunoId, cancellationToken))
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Celo,
                new List<string>()
                {
                    "CELO-VACUNO-ID-NOT_EXISTS",
                }
            );

        var codigo = $"C{DateTime.UtcNow:yyMMddHHmmss}";
        if (await repository.ExistsCodigoAsync(codigo, cancellationToken))
            codigo = $"C{DateTime.UtcNow:yyMMddHHmmssff}";

        var utcNow = DateTime.UtcNow;

        var saved = await _unitOfWork.ExecuteInTransactionAsync(
            operation: ct =>
            {
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

                return repository.AddAsync(celo, ct);
            },
            cancellationToken: cancellationToken);

        return new CreateCeloOutput(
            Id: saved.Id,
            Codigo: saved.Codigo,
            FechaHora: saved.FechaHora);
    }
}
