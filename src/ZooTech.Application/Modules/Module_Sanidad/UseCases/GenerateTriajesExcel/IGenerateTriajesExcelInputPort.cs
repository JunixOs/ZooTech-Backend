namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;

public interface IGenerateTriajesExcelInputPort
{
    Task<GenerateTriajesExcelOutput> HandleAsync(GenerateTriajesExcelQuery query, CancellationToken cancellationToken);
}
