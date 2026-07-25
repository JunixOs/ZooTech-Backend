using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetHistorialCeloPorVacuno;

public sealed class GetHistorialCeloPorVacunoInteractor(ICeloDetalleRepository repository) : IGetHistorialCeloPorVacunoInputPort
{
    public async Task<GetHistorialCeloPorVacunoOutput> HandleAsync(GetHistorialCeloPorVacunoCommand command, CancellationToken cancellationToken = default)
    {
        var detalle = await repository.GetDetallePorVacunoAsync(command.CodigoVacuno, command.RegistroId, cancellationToken);
        return new GetHistorialCeloPorVacunoOutput(
            detalle.Encargado,
            detalle.Historial.Select(item => new CeloHistorialCeloPorVacunoDto(
                item.Numero, item.FechaHora, item.Resultado)).ToList(),
            new CeloResumenReproductivoDto(
                detalle.Resumen.Celos,
                detalle.Resumen.Embarazos,
                detalle.Resumen.Fecundaciones,
                detalle.Resumen.Crias,
                detalle.Resumen.Montas,
                detalle.Resumen.Inseminaciones,
                detalle.Resumen.UltimoParto));
    }
}
