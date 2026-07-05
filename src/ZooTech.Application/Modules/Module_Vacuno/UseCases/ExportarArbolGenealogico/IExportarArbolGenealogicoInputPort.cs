namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;

public interface IExportarArbolGenealogicoInputPort
{
    Task<byte[]> HandleAsync(long vacunoId, ExportarArbolGenealogicoCommand command, CancellationToken cancellationToken = default);
}
