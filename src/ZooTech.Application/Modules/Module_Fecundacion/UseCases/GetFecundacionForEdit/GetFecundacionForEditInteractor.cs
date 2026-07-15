using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;

public sealed class GetFecundacionForEditInteractor : IGetFecundacionForEditInputPort
{
    private readonly IFecundacionRepository _repository;

    public GetFecundacionForEditInteractor(IFecundacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetFecundacionForEditOutput> HandleAsync(
        GetFecundacionForEditCommand cmd,
        CancellationToken cancellationToken = default)
    {
        var data = await _repository.GetForEditAsync(cmd.Id, cancellationToken)
            ?? throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Fecundacion,
                $"No se encontró la fecundación con ID {cmd.Id}."
            );

        return new GetFecundacionForEditOutput(
            data.Id,
            data.Codigo,
            data.TipoFecundacionCode,
            data.VacunoReceptorId,
            data.VacunoReceptorCodigo,
            data.VacunoReceptorNombre,
            data.TipoDonante,
            data.VacunoDonanteId,
            data.VacunoDonanteCodigo,
            data.VacunoDonanteNombre,
            data.ExternoDonanteId,
            data.ExternoDonanteNombre,
            data.FechaProcedimiento,
            data.ResponsableNombre,
            data.ResultadoCode,
            data.EstadoFecundacionCode,
            data.ObservacionesVeterinarias,
            data.CodigoSemen,
            data.CodigoEmbrion,
            data.CreadoEn,
            data.ActualizadoEn);
    }
}
