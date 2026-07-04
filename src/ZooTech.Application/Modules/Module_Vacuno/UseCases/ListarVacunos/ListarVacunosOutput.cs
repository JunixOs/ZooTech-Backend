namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public sealed record VacunoItemDto(
    long Id,
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    DateOnly FechaRegistro,
    string RazaCode,
    string SexoCode,
    string? Procedencia,
    bool IsDeleted);

public sealed record ListarVacunosOutput(IReadOnlyList<VacunoItemDto> Items);
