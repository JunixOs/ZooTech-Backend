using ZooTech.Domain.Ganaderia.Module_Fecundacion.Entities;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Entities;

namespace ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;

public interface IFecundacionRepository
{
    Task<(List<FecundacionListItem> Items, int TotalCount)> GetPagedAsync(
    string? query, DateTime? fechaDesde, DateTime? fechaHasta, string? resultado,
    int page, int limit, CancellationToken cancellationToken = default);
    Task<Fecundacion> AddAsync(Fecundacion fecundacion, CancellationToken cancellationToken = default);

    Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken = default);

    Task<bool> ExistsCeloAsync(long celoId, CancellationToken cancellationToken = default);

    Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default);

    Task<long> GetOrCreateResponsableByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<FecundacionEditData?> GetForEditAsync(long id, CancellationToken cancellationToken = default);
    Task<FecundacionOptionsData> GetOptionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FecundacionVacunoOptionData>> SearchVacunosAsync(
        string? sexo,
        string? query,
        bool soloDisponibles = false,
        long? excluirFecundacionId = null,
        CancellationToken cancellationToken = default);
    Task<FecundacionUpdateData?> UpdateAsync(long id, FecundacionUpdateValues values, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, string razon, CancellationToken cancellationToken = default);
    Task<bool> HasCriaAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> HasActiveFecundacionAsync(long? fecundacionId, long receptorId, CancellationToken cancellationToken = default);
}

public sealed record FecundacionOptionData(string Code, string Nombre, string? Descripcion);
public sealed record FecundacionVacunoOptionData(long Id, string Codigo, string Nombre, string Sexo);
public sealed record FecundacionOptionsData(
    IReadOnlyList<FecundacionOptionData> Tipos,
    IReadOnlyList<FecundacionOptionData> Resultados,
    IReadOnlyList<FecundacionOptionData> Estados);
public sealed record FecundacionEditData(
    long Id, string Codigo, string TipoFecundacionCode, long VacunoReceptorId, string VacunoReceptorCodigo,
    string VacunoReceptorNombre, string TipoDonante, long? VacunoDonanteId, string? VacunoDonanteCodigo,
    string? VacunoDonanteNombre, long? ExternoDonanteId, string? ExternoDonanteNombre, DateOnly FechaProcedimiento,
    string ResponsableNombre, string ResultadoCode, string EstadoFecundacionCode, string? ObservacionesVeterinarias,
    string? CodigoSemen, string? CodigoEmbrion, DateTime CreadoEn, DateTime ActualizadoEn);
public sealed record FecundacionUpdateValues(
    string TipoFecundacionCode, long VacunoReceptorId, string TipoDonante, long? VacunoDonanteId,
    string? ExternoDonanteNombre, DateOnly FechaProcedimiento, string ResponsableNombre, string ResultadoCode,
    string EstadoFecundacionCode, string? ObservacionesVeterinarias, string? CodigoSemen, string? CodigoEmbrion);
public sealed record FecundacionUpdateData(long Id, string Codigo, string ResultadoCode, string EstadoFecundacionCode, string? Warning);
