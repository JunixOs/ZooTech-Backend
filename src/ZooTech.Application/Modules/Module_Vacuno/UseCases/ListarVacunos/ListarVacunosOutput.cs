namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public sealed record VacunoItemDto(
    long Id,
    string Codigo,
    string Nombre,
    string RazaCode,
    string SexoCode);

public sealed record ListarVacunosOutput(IReadOnlyList<VacunoItemDto> Items);
