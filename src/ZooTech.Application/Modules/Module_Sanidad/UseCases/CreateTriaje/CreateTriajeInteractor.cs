using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Common.Interfaces;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;

internal static class TriajeReferenceValidator
{
    public static async Task EnsureReferencesExistAsync(
        ITriajeRepository repository,
        long vacunoId,
        long? encargadoUsuarioId,
        string tipoPesoCode,
        CancellationToken cancellationToken)
    {
        if (!await repository.ExistsVacunoAsync(vacunoId, cancellationToken))
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Triaje,
                new List<string>()
                {
                    "TRIAJE-TRIAJE-VACUNO-NOT_EXISTS"
                },
                "El vacuno indicado no existe."
                );

        if (encargadoUsuarioId.HasValue && !await repository.ExistsUsuarioAsync(encargadoUsuarioId.Value, cancellationToken))
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Triaje,
                new List<string>()
                {
                    "TRIAJE-TRIAJE-ENCARGADO_USUARIO_ID-NOT_EXISTS"
                },
                "El usuario encargado indicado no existe."
            );

        if (!await repository.ExistsTipoPesoAsync(tipoPesoCode, cancellationToken))
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Triaje,
                new List<string>()
                {
                    "TRIAJE-TRIAJE-TIPO_PESO-NOT_EXISTS"
                },
                "El tipo de peso indicado no existe."
            );
    }
}

public sealed class CreateTriajeInteractor : ICreateTriajeInputPort
{
    private readonly ITriajeRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IEstadoRegistroRepository _estadoRegistroRepository;

    public CreateTriajeInteractor(
        ITriajeRepository repository,
        IDateTimeProvider dateTimeProvider,
        IEstadoRegistroRepository estadoRegistroRepository
    )
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _estadoRegistroRepository = estadoRegistroRepository;
    }

    public async Task<CreateTriajeOutput> Handle(CreateTriajeCommand command, CancellationToken cancellationToken = default)
    {
        await TriajeReferenceValidator.EnsureReferencesExistAsync(
            _repository, command.VacunoId, command.EncargadoUsuarioId, command.TipoPesoCode, cancellationToken);

        var codigo = await _repository.GenerateCodigoAsync(cancellationToken);
        var utcNow = _dateTimeProvider.ServerNow;
        var estadoActivo = await _estadoRegistroRepository.GetActiveCodeAsync(cancellationToken);

        var triaje = Triaje.CreateNew(
            codigo: codigo,
            fechaHora: utcNow,
            vacunoId: command.VacunoId,
            tipoPesoCode: command.TipoPesoCode,
            pesoKg: command.PesoKg,
            observaciones: command.Observaciones,
            estadoRegistroCode: estadoActivo,
            encargadoUsuarioId: command.EncargadoUsuarioId,
            utcNow: utcNow);

        var saved = await _repository.AddAsync(triaje, cancellationToken);

        return new CreateTriajeOutput(
            saved.Id, saved.Codigo, saved.FechaHora, saved.VacunoId,
            saved.TipoPesoCode, saved.PesoKg, saved.Observaciones,
            saved.EstadoRegistroCode, saved.EncargadoUsuarioId, saved.CreatedAt);
    }
}
