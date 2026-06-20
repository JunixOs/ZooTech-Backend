using ZooTech.Domain.Module_Vacuno.Entities;

namespace ZooTech.Domain.Module_Vacuno.Interfaces;

public interface IRegistroVacunoReadRepository
{
    Task<Vacuno?> ObtenerRegistroAsync(
        long vacunoId,
        CancellationToken cancellationToken = default);

    Task<string?> ObtenerNombreCatalogoAsync(string tipoCatalogo, string codigo, CancellationToken cancellationToken = default);

    Task<string?> ObtenerCodigoVacunoAsync(long id, CancellationToken cancellationToken = default);

    Task<(string Nombre, string? Distrito, string? Provincia, string? Departamento)?> ObtenerDetallesGranjaAsync(long granjaId, CancellationToken cancellationToken = default);

    Task<(decimal? PrecioCompra, string? Proveedor, DateOnly? FechaAdquisicion)?> ObtenerDetallesAdquisicionAsync(long vacunoId, CancellationToken cancellationToken = default);

    Task<(string? EstadoCode, string? EstadoNombre, DateOnly? FechaEstado, string? Motivo)?> ObtenerEstadoActualAsync(long vacunoId, CancellationToken cancellationToken = default);

    Task<string?> ObtenerUtilizacionActualAsync(long vacunoId, CancellationToken cancellationToken = default);

    Task<(long? Id, string? NombreOriginal, string? NombreAlmacenado, string? RutaArchivo, string? Extension, long? TamanoBytes)?> ObtenerFotoPrincipalAsync(long vacunoId, CancellationToken cancellationToken = default);

    Task<string?> ObtenerNombreUsuarioAsync(long? usuarioId, CancellationToken cancellationToken = default);
}
