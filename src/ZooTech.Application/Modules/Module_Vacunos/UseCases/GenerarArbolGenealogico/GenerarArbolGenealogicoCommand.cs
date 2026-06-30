namespace ZooTech.Application.Modules.Module_Vacunos.UseCases.GenerarArbolGenealogico;

public class GenerarArbolGenealogicoCommand
{
    public long VacunoId { get; set; }
    public int Niveles { get; set; } = 4;
}
