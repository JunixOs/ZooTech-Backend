using ZooTech.Domain.Module_Sanidad.Entities;

namespace ZooTech.Domain.Module_Sanidad.Interfaces;

public interface ITriajeRepository
{
    Task<Triaje?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Triaje> Items, int Total)> GetAllAsync(
        int pagina,
        int tamano,
        string? fecha = null,
        string? fechaDesde = null,
        string? fechaHasta = null,
        string? codigo = null,
        string? nombre = null,
        string? tipoPeso = null,
        decimal? pesoKg = null,
        CancellationToken cancellationToken = default);
    Task<Triaje> AddAsync(Triaje triaje, CancellationToken cancellationToken = default);
    Task<Triaje> UpdateAsync(Triaje triaje, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
    Task<string> GenerateCodigoAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<TriajeHistorialItem>> GetHistorialByVacunoIdAsync(long vacunoId, string? fechaDesde = null, string? fechaHasta = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<TriajeDetallePorVacunoItem>> GetDetallesByVacunoIdAsync(long vacunoId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TriajeHistorialItem>> GetHistorialGeneralAsync(string? fechaDesde = null, string? fechaHasta = null, CancellationToken cancellationToken = default);
     
    Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken = default);
    Task<bool> ExistsUsuarioAsync(long usuarioId, CancellationToken cancellationToken = default);
    Task<bool> ExistsTipoPesoAsync(string tipoPesoCode, CancellationToken cancellationToken = default);

}
