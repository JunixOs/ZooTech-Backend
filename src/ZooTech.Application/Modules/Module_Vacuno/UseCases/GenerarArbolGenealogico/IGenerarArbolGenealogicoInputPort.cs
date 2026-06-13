namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;

public interface IGenerarArbolGenealogicoInputPort
{
    Task<GenerarArbolGenealogicoOutput> HandleAsync(long vacunoId, GenerarArbolGenealogicoCommand command, CancellationToken cancellationToken = default);
}
