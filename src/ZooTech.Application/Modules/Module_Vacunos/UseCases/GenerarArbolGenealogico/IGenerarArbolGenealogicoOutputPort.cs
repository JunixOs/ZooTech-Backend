namespace ZooTech.Application.Modules.Module_Vacunos.UseCases.GenerarArbolGenealogico;

public interface IGenerarArbolGenealogicoOutputPort
{
    Task Ok(GenerarArbolGenealogicoOutput output);
    Task NotFound(string message);
    Task Error(string code, string message);
}
