using FluentValidation;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Reproduccion.Entities;
using ZooTech.Domain.Module_Reproduccion.Interfaces;

namespace ZooTech.Application.Modules.Module_Reproduccion.UseCases.RegistrarFecundacion;

public sealed class RegistrarFecundacionInteractor : IRegistrarFecundacionInputPort
{
    private readonly IFecundacionRepository _repository;
    private readonly IVacunoReproduccionRepository _vacunoRepository;
    private readonly IValidator<RegistrarFecundacionCommand> _validator;

    public RegistrarFecundacionInteractor(
        IFecundacionRepository repository,
        IVacunoReproduccionRepository vacunoRepository,
        IValidator<RegistrarFecundacionCommand> validator)
    {
        _repository = repository;
        _vacunoRepository = vacunoRepository;
        _validator = validator;
    }

    public async Task<RegistrarFecundacionOutput> HandleAsync(RegistrarFecundacionCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        // Validaciones de negocio complejas
        if (!await _vacunoRepository.ExistsAsync(command.VacunoReceptorId, cancellationToken))
        {
            throw new NotFoundException($"No se encontró el vacuno receptor con ID {command.VacunoReceptorId}.");
        }

        if (!await _vacunoRepository.IsHembraAsync(command.VacunoReceptorId, cancellationToken))
        {
            throw new ConflictException("El vacuno receptor debe ser una hembra.");
        }

        if (!await _vacunoRepository.IsVivoAsync(command.VacunoReceptorId, cancellationToken))
        {
            throw new ConflictException("El vacuno receptor debe estar en estado VIVO.");
        }

        if (await _repository.HasPendingFecundacionAsync(command.VacunoReceptorId, cancellationToken))
        {
            throw new ConflictException("El vacuno receptor ya tiene una fecundación PENDIENTE activa.");
        }

        // Si es donante interno, validar que exista
        if (command.VacunoDonanteId.HasValue)
        {
            if (!await _vacunoRepository.ExistsAsync(command.VacunoDonanteId.Value, cancellationToken))
            {
                throw new NotFoundException($"No se encontró el vacuno donante con ID {command.VacunoDonanteId.Value}.");
            }
        }

        var newCode = await _repository.GenerateNextCodeAsync(cancellationToken);

        // Si se envió un nombre externo, ExternoDonanteId lo resolverá el repositorio al buscar/insertar en la tabla reproductor_externo.
        // Como el repositorio no maneja lógica compleja de creación de externos si no le pasamos el nombre,
        // pasamos null a ExternoDonanteId y le enviamos el NombreMachoExterno para que la capa de persistencia lo gestione.
        
        var fecundacion = Fecundacion.CreateNew(
            newCode,
            command.TipoFecundacionCode,
            command.VacunoReceptorId,
            command.VacunoDonanteId,
            null, // Se resolverá en base al NombreMachoExterno
            command.NombreMachoExterno,
            command.FechaProcedimiento,
            command.ResponsableId,
            command.CodigoSemen,
            command.CodigoEmbrion,
            command.Observaciones,
            command.CurrentUserId
        );

        await _repository.AddAsync(fecundacion, cancellationToken);

        return new RegistrarFecundacionOutput(
            fecundacion.Id,
            fecundacion.Codigo,
            fecundacion.TipoFecundacionCode,
            fecundacion.VacunoReceptorId,
            fecundacion.ResultadoCode,
            fecundacion.FechaProcedimiento
        );
    }
}
