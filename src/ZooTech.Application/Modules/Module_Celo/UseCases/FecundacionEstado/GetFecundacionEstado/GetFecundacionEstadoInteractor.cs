using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.GetFecundacionEstado;

public sealed class GetFecundacionEstadoInteractor : IGetFecundacionEstadoInputPort
{
    private readonly IFecundacionEstadoRepository repository;

    public GetFecundacionEstadoInteractor(IFecundacionEstadoRepository repository)
    {
        this.repository = repository;
    }

    public async Task<GetFecundacionEstadoOutput> HandleAsync(
        GetFecundacionEstadoCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.VacunoId <= 0)
        {
            throw new FecundacionEstadoValidationException(
                new Dictionary<string, string>
                {
                    ["vacunoId"] = "El identificador del vacuno debe ser mayor a cero."
                });
        }

        var snapshot = await repository.GetByVacunoIdAsync(command.VacunoId, cancellationToken)
            ?? throw new NotFoundException("No se encontro el vacuno solicitado.");

        if (!snapshot.EsHembra)
        {
            throw new ConflictException("El estado de fecundacion solo aplica a vacunos hembra.");
        }

        return new GetFecundacionEstadoOutput(
            snapshot.VacunoId,
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
