
namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

public interface IGetOrdeniosExcelInputPort
{
    Task<GenerateOrdeniosExcelOutput> Handle(GenerateOrdeniosComparationExcelQuery query, CancellationToken cancellationToken);
}


