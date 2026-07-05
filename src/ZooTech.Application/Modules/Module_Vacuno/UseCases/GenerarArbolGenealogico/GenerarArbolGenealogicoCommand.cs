namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;

public class GenerarArbolGenealogicoCommand
{
    public long VacunoId { get; set; }
    public int Niveles { get; set; } = 4;
}
