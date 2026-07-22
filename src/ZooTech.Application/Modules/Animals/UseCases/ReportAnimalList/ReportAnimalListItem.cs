namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public sealed record ReportAnimalListItem(
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    string TipoAdquisicion,
    string Raza,
    string Color,
    string Sexo,
    string Granja,
    string Estado,
    DateOnly FechaRegistro);
