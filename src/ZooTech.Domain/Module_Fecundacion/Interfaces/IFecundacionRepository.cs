namespace ZooTech.Domain.Module_Fecundacion.Interfaces;

public interface IFecundacionRepository
{
    Task<FecundacionEditData?> GetForEditAsync(long id, CancellationToken cancellationToken = default);

    Task<FecundacionOptionsData> GetOptionsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FecundacionVacunoOptionData>> SearchVacunosAsync(
        string? sexo,
        string? query,
        CancellationToken cancellationToken = default);

    Task<FecundacionUpdateData?> UpdateAsync(
        long id,
        FecundacionUpdateValues values,
        CancellationToken cancellationToken = default);
}

public sealed record FecundacionOptionData(string Code, string Nombre, string? Descripcion);

public sealed record FecundacionVacunoOptionData(long Id, string Codigo, string Nombre, string Sexo);

public sealed record FecundacionOptionsData(
    IReadOnlyList<FecundacionOptionData> Tipos,
    IReadOnlyList<FecundacionOptionData> Resultados,
    IReadOnlyList<FecundacionOptionData> Estados);

public sealed record FecundacionEditData(
    long Id,
    string Codigo,
    string TipoFecundacionCode,
    long VacunoReceptorId,
    string VacunoReceptorCodigo,
    string VacunoReceptorNombre,
    string TipoDonante,
    long? VacunoDonanteId,
    string? VacunoDonanteCodigo,
    string? VacunoDonanteNombre,
    long? ExternoDonanteId,
    string? ExternoDonanteNombre,
    DateOnly FechaProcedimiento,
    string ResponsableNombre,
    string ResultadoCode,
    string EstadoFecundacionCode,
    string? ObservacionesVeterinarias,
    string? CodigoSemen,
    string? CodigoEmbrion,
    DateTime CreadoEn,
    DateTime ActualizadoEn);

public sealed record FecundacionUpdateValues(
    string TipoFecundacionCode,
    long VacunoReceptorId,
    string TipoDonante,
    long? VacunoDonanteId,
    string? ExternoDonanteNombre,
    DateOnly FechaProcedimiento,
    string ResponsableNombre,
    string ResultadoCode,
    string EstadoFecundacionCode,
    string? ObservacionesVeterinarias,
    string? CodigoSemen,
    string? CodigoEmbrion);

public sealed record FecundacionUpdateData(
    long Id,
    string Codigo,
    string ResultadoCode,
    string EstadoFecundacionCode,
    string? Warning);
