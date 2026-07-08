namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;

public sealed record UpdateVacunoCommand(
    string Nombre,
    DateOnly FechaNacimiento,
    string TipoAdquisicionCode,
    string RazaCode,
    string ColorCode,
    string SexoCode,
    long? PadreId,
    long? MadreId,
    long GranjaId,
    string? Observaciones,
    decimal? PrecioCompra,
    string? AptoPara);
