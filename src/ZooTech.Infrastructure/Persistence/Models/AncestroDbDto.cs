namespace ZooTech.Infrastructure.Persistence.Models;

public class AncestroDbDto
{
    public long Id { get; set; }
    public string Codigo { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public string Raza { get; set; } = default!;
    public string Sexo { get; set; } = default!;
    public long? PadreId { get; set; }
    public long? MadreId { get; set; }
    public int Nivel { get; set; }

    // Referencias para el árbol en memoria
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public AncestroDbDto? Padre { get; set; }
    
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public AncestroDbDto? Madre { get; set; }
}
