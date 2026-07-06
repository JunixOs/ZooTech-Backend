namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;

public interface IGenerarArbolGenealogicoOutputPort
{
    Task Ok(GenerarArbolGenealogicoOutput output);
    Task NotFound(string message);
    Task Error(string code, string message);
}
