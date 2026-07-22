using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Domain.Module_Fecundacion.Entities;
using ZooTech.Domain.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;

public sealed class CreateFecundacionInteractor : ICreateFecundacionInputPort
{
    private readonly IFecundacionRepository _fecundacionRepository;
    public CreateFecundacionInteractor(
        IFecundacionRepository fecundacionRepository
    )
    {
        _fecundacionRepository = fecundacionRepository;
    }

    public async Task<CreateFecundacionOutput> HandleAsync(
        CreateFecundacionCommand command,
        CancellationToken cancellationToken = default)
    {
        // Validar que el vacuno receptor exista
        if (!await _fecundacionRepository.ExistsVacunoAsync(command.VacunoReceptorId, cancellationToken))
            throw new FecundacionVacunoNotFoundException();

        // Validar que no tenga otra fecundación activa con resultado 'Pendiente de confirmación'
        if (await _fecundacionRepository.HasActiveFecundacionAsync(null, command.VacunoReceptorId, cancellationToken))
            throw new FecundacionPendingActiveException();

        // Validar que el celo exista (si se proporcionó)
        if (command.CeloRegistroId.HasValue && !await _fecundacionRepository.ExistsCeloAsync(command.CeloRegistroId.Value, cancellationToken))
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Fecundacion,
                message: $"El registro de celo con ID {command.CeloRegistroId.Value} no existe."
            );

        // Validar que el vacuno donante exista (si no es macho externo y se proporcionó)
        if (!command.MachoExterno && command.VacunoDonanteId.HasValue)
        {
            if (!await _fecundacionRepository.ExistsVacunoAsync(command.VacunoDonanteId.Value, cancellationToken))
                throw new ConflictException(
                    ScopeName.Application,
                    ModuleName.Fecundacion,
                    message: $"El vacuno donante con ID {command.VacunoDonanteId.Value} no existe."
                );
        }

        // Obtener o crear responsable
        var responsableId = await _fecundacionRepository.GetOrCreateResponsableByNameAsync(command.ResponsableName, cancellationToken);

        // Generar código único para la fecundación (Límite de la base de datos: 15 caracteres)
        var codigo = $"F{DateTime.UtcNow:yyMMddHHmmss}";
        if (await _fecundacionRepository.ExistsCodigoAsync(codigo, cancellationToken))
            codigo = $"F{DateTime.UtcNow:yyMMddHHmmssff}";

        var utcNow = DateTime.UtcNow;

        // Crear la entidad de dominio
        var fecundacion = Fecundacion.CreateNew(
            codigo: codigo,
            tipoFecundacionCode: command.TipoFecundacionCode,
            vacunoReceptorId: command.VacunoReceptorId,
            celoRegistroId: command.CeloRegistroId,
            fechaProcedimiento: command.FechaProcedimiento,
            responsableId: responsableId,
            resultadoCode: command.ResultadoCode,
            observacionesVeterinarias: command.ObservacionesVeterinarias,
            actorUsuarioId: command.CreatedById,
            utcNow: utcNow,
            machoExterno: command.MachoExterno,
            machoExternoNombre: command.MachoExternoNombre,
            vacunoDonanteId: command.VacunoDonanteId,
            codigoSemen: command.CodigoSemen,
            codigoEmbrion: command.CodigoEmbrion);

        // Persistir en base de datos
        var saved = await _fecundacionRepository.AddAsync(fecundacion, cancellationToken);

        return new CreateFecundacionOutput(
            Id: saved.Id,
            Codigo: saved.Codigo,
            FechaProcedimiento: saved.FechaProcedimiento);
    }
}
