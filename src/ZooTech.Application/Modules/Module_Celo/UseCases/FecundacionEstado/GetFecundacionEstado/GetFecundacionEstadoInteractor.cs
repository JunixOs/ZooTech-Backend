using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.GetFecundacionEstado;

public sealed class GetFecundacionEstadoInteractor : IGetFecundacionEstadoInputPort
{
    private readonly IFecundacionEstadoRepository repository;

    public GetFecundacionEstadoInteractor(IFecundacionEstadoRepository repository)
    {
        this.repository = repository;
    }

    public async Task<GetFecundacionEstadoOutput> HandleAsync(
        GetFecundacionEstadoQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.VacunoId <= 0)
        {
            throw new FecundacionEstadoValidationException(
                new List<string>
                {
                    "FECUNDACION-GET_FECUNDACION_ESTADO-VACUNO_ID-INVALID"
                },
                "El identificador del vacuno debe ser mayor a cero."
                );
        }

        var snapshot = await repository.GetByVacunoIdAsync(query.VacunoId, cancellationToken)
            ?? throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Fecundacion,
                "No se encontro el vacuno solicitado."
            );

        if (!snapshot.EsHembra)
        {
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Fecundacion,
                message: "El estado de fecundacion solo aplica a vacunos hembra."
            );
        }

        return new GetFecundacionEstadoOutput(
            snapshot.VacunoId,
            snapshot.FecundacionId,
            snapshot.CodigoVacuno,
            snapshot.NombreVacuno,
            snapshot.EstadoActual,
            snapshot.DisponibleNuevaFecundacion,
            snapshot.UltimaActualizacion,
            snapshot.CodigoFecundacion,
            snapshot.TipoFecundacion,
            snapshot.ToroDonante,
            snapshot.Responsable,
            snapshot.FechaProcedimiento,
            snapshot.Resultado,
            snapshot.Observaciones);
    }
}
