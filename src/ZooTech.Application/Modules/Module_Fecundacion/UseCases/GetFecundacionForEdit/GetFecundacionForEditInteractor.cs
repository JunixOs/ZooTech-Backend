using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Fecundacion.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;

public sealed class GetFecundacionForEditInteractor : IGetFecundacionForEditInputPort
{
    private readonly IFecundacionRepository _repository;

    public GetFecundacionForEditInteractor(IFecundacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetFecundacionForEditOutput> HandleAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var data = await _repository.GetForEditAsync(id, cancellationToken)
            ?? throw new NotFoundException($"No se encontró la fecundación con ID {id}.");

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
