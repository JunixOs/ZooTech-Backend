namespace ZooTech.InterfaceAdapters.DTOs.Responses;

public sealed class DeleteAnimalResponse
{
    public long Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string MotivoEliminacion { get; set; } = string.Empty;

    public long? EliminadoPor { get; set; }

    public DateTime FechaEliminacion { get; set; }

    public bool ResueltoComoDuplicado { get; set; }
}
