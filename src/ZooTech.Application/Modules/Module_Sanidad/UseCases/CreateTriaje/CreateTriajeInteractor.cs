using FluentValidation;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;

public sealed class CreateTriajeInteractor : ICreateTriajeInputPort
{
    private readonly ITriajeRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IValidator<CreateTriajeCommand> _validator;

    public CreateTriajeInteractor(ITriajeRepository repository, IDateTimeProvider dateTimeProvider, IValidator<CreateTriajeCommand> validator)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _validator = validator;
    }

    public async Task<CreateTriajeOutput> HandleAsync(CreateTriajeCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);
        var codigo = await _repository.GenerateCodigoAsync(cancellationToken);
        var utcNow = _dateTimeProvider.ServerNow;

        var triaje = Triaje.CreateNew(
            codigo: codigo,
            fechaHora: utcNow,
            vacunoId: command.VacunoId,
            tipoPesoCode: command.TipoPesoCode,
            pesoKg: command.PesoKg,
            observaciones: command.Observaciones,
            estadoRegistroCode: command.EstadoRegistroCode,
            encargadoUsuarioId: command.EncargadoUsuarioId,
            utcNow: utcNow);

        var saved = await _repository.AddAsync(triaje, cancellationToken);

        return new CreateTriajeOutput(
            saved.Id, saved.Codigo, saved.FechaHora, saved.VacunoId,
            saved.TipoPesoCode, saved.PesoKg, saved.Observaciones,
            saved.EstadoRegistroCode, saved.EncargadoUsuarioId, saved.CreatedAt);
    }
}
