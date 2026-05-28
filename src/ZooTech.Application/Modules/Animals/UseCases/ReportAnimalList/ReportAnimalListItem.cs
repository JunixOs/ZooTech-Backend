namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public sealed record ReportAnimalListItem(
    string Codigo,
    string Nombre,
    string Raza,
    string Sexo,
    string Procedencia,
    string Estado,
    DateOnly FechaRegistro);
