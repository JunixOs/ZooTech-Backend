namespace ZooTech.Application.Modules.Module_Vacunos.UseCases.GenerarArbolGenealogico;

public interface IGenerarArbolGenealogicoInputPort
{
    Task Handle(GenerarArbolGenealogicoCommand command);
}
