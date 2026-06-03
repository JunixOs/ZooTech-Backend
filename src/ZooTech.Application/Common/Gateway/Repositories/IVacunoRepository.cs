using ZooTech.Domain.Module_Vacuno.Entities;

namespace ZooTech.Application.Common.Gateway.Repositories;

/// <summary>
/// Contrato del repositorio de Vacuno.
/// Infrastructure implementa esta interfaz; Application solo la consume.
/// </summary>
public interface IVacunoRepository
{
    /// <summary>Verifica si ya existe un vacuno con el mismo código en el tenant actual.</summary>
    Task<bool> ExisteCodigoAsync(string codigo, CancellationToken ct = default);

    /// <summary>Persiste el vacuno y todas sus entidades relacionadas en una transacción.</summary>
    Task<Vacuno> RegistrarAsync(
        Vacuno vacuno,
        VacunoAdquisicion adquisicion,
        VacunoUtilizacionHistorial utilizacion,
        VacunoFoto? foto,
        CancellationToken ct = default);

    Task<IReadOnlyList<Vacuno>> ListarAsync(int page, int limit, string? q, string? estado, CancellationToken ct = default);

    Task<int> ContarAsync(string? q, string? estado, CancellationToken ct = default);

    Task<Vacuno?> ObtenerPorIdAsync(int id, CancellationToken ct = default);

    Task<Vacuno?> ObtenerPorCodigoAsync(string codigo, CancellationToken ct = default);

    Task<bool> ExisteRegistroDuplicadoAsync(
        string codigoExcluido,
        EditarVacunoData data,
        CancellationToken ct = default);

    Task<Vacuno?> ActualizarAsync(
        string codigo,
        EditarVacunoData data,
        CancellationToken ct = default);

    Task<IReadOnlyList<UbigeoOption>> ListarUbigeoAsync(CancellationToken ct = default);
}
