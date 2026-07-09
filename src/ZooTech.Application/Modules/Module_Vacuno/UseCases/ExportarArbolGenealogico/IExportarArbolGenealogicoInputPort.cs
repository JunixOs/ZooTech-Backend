namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;

public interface IExportarArbolGenealogicoInputPort
{
    Task<ExportarArbolGenealogicoOutput> HandleAsync(ExportarArbolGenealogicoCommand command, CancellationToken cancellationToken = default);
}
