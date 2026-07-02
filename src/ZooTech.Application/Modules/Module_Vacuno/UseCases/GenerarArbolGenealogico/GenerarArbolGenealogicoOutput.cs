namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;

public class GenerarArbolGenealogicoOutput
{
    public VacunoNodoDto Data { get; set; } = default!;
}

public class VacunoNodoDto
{
    public long Id { get; set; }
    public string Codigo { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public string Raza { get; set; } = default!;
    public string Sexo { get; set; } = default!;
    public int Nivel { get; set; }
    
    public VacunoNodoDto? Padre { get; set; }
    public VacunoNodoDto? Madre { get; set; }
}
