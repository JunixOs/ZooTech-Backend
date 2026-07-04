
namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

public interface IGetOrdeniosExcelInputPort
{
    Task<GenerateOrdeniosExcelOutput> HandleAsync(GenerateOrdeniosComparationExcelQuery query, CancellationToken cancellationToken);
}


