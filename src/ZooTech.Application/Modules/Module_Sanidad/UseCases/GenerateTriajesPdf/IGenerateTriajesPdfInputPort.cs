namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;

public interface IGenerateTriajesPdfInputPort
{
    Task<GenerateTriajesPdfOutput> HandleAsync(GenerateTriajesPdfQuery query, CancellationToken cancellationToken);
}
