using ZooTech.Domain.Module_Sanidad.Entities;

namespace ZooTech.Domain.Module_Sanidad.Interfaces;

public interface ITriajeRepository
{
    Task<Triaje?> GetByIdAsync(long id);
    Task<(IEnumerable<Triaje> Items, int Total)> GetAllAsync(
     int pagina,
     int tamano,
     string? fecha = null,
     string? codigo = null,
     string? nombre = null,
     string? tipoPeso = null,
     decimal? pesoKg = null);
    Task AddAsync(Triaje triaje);
    Task UpdateAsync(Triaje triaje);
    Task DeleteAsync(long id);
    Task<string> GenerateCodigoAsync();

    // Tipo peso
    Task<IEnumerable<TipoPeso>> GetAllTipoPesosAsync();
    // Vacuno id codigo nombre
    Task<IEnumerable<VacunoOption>> GetAllVacunosAsync();

    Task<IEnumerable<TriajeHistorialItem>> GetHistorialByVacunoIdAsync(long vacunoId);
    Task<IEnumerable<Triaje>> GetGeneralReportAsync(DateTime? startDate, DateTime? endDate);

    // Reportes agregados
    Task<ResumenSanidad> GetResumenAsync(DateTime? fechaInicio, DateTime? fechaFin);
    Task<IEnumerable<PesoPromedioItem>> GetPesoPromedioPorPeriodoAsync(DateTime? fechaInicio, DateTime? fechaFin);
    Task<IEnumerable<TipoPesoCountItem>> GetDistribucionTipoPesoAsync(DateTime? fechaInicio, DateTime? fechaFin);
}

public class ResumenSanidad
{
    public int TotalTriajes { get; set; }
    public decimal PesoPromedioKg { get; set; }
    public decimal PesoMinimoKg { get; set; }
    public decimal PesoMaximoKg { get; set; }
    public int TotalVacunosEvaluados { get; set; }
    public List<TipoPesoCountItem> DistribucionTipoPeso { get; set; } = new();
}

public class TipoPesoCountItem
{
    public string Code { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}

public class PesoPromedioItem
{
    public DateTime Fecha { get; set; }
    public decimal PesoPromedio { get; set; }
    public int CantidadRegistros { get; set; }
}