namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;

public sealed record ArbolVacunoDto(
    long Id,
    string Codigo,
    string Nombre,
    string SexoCode,
    long? PadreId,
    long? MadreId);

public sealed record GenerarArbolGenealogicoOutput(IReadOnlyList<ArbolVacunoDto> Arbol);
