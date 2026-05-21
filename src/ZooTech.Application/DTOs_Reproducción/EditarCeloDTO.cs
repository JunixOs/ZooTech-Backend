using System.Collections.Generic;

namespace ZooTech.Application.DTOs.Reproduccion
{
    public class EditarCeloDTO
    {
        public long Id { get; set; }
        public string? Observaciones { get; set; }
        public List<string> CaracteristicaCodes { get; set; } = new();
    }
}