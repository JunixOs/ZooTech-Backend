namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;

public sealed record GetAllTriajesOutput(
    IReadOnlyList<TriajeItemOutput> Items,
    int TotalRegistros,
    int Pagina,
    int Tamano,
    int TotalPaginas);

public sealed record TriajeItemOutput(
    long Id,
    string Codigo,
    DateTime FechaHora,
    long VacunoId,
    string VacunoNombre,
    string TipoPesoCode,
    decimal PesoKg,
    string? Observaciones,
    string EstadoRegistroCode,
    long? EncargadoUsuarioId,
    DateTime CreatedAt);
