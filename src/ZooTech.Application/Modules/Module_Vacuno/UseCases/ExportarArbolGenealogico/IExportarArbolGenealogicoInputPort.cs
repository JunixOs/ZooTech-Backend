using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;

public interface IExportarArbolGenealogicoInputPort
    : IRequestHandler<ExportarArbolGenealogicoQuery , ExportarArbolGenealogicoOutput>
{
}
