namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

public interface IGetOrdeniosPdfInputPort
{
    Task<GenerateOrdeniosPdfOutput> HandleAsync(GenerateOrdeniosPdfQuery query, CancellationToken cancellationToken);
}
