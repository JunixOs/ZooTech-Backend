namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;

public sealed record GetHistorialByVacunoIdOutput(IReadOnlyList<HistorialTriajeItemOutput> Items);

public sealed record HistorialTriajeItemOutput(
    long Id,
    DateTime FechaHora,
    string TipoPesoCode,
    decimal PesoKg);
