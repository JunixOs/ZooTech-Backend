using System;
using System.Collections.Generic;

namespace ZooTech.Application.DTOs.Reproduccion
{
    public class RegistrarCeloDTO
    {
        public long VacunoId { get; set; }
        public long EncargadoUsuarioId { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Observaciones { get; set; }
        public List<string> CaracteristicaCodes { get; set; } = new();
    }
}