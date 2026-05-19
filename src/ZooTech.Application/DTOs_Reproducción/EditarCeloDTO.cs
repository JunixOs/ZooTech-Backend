namespace ZooTech.Application.DTOs.Reproduccion
{
    public class EditarCeloDTO
    {
        public long Id { get; set; }

        public string? Observaciones { get; set; }

        // IDs de características seleccionadas
        public List<long> CaracteristicaIds { get; set; } = new();
    }
}
