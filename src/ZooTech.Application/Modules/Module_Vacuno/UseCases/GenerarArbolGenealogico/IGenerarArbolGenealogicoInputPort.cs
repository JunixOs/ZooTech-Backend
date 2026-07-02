namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;

public interface IGenerarArbolGenealogicoInputPort
{
    Task Handle(GenerarArbolGenealogicoCommand command);
}
