namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;

public sealed record GetAllVacunosSanidadOutput(IReadOnlyList<VacunoSanidadItemOutput> Items);

public sealed record VacunoSanidadItemOutput(long Id, string Codigo, string Nombre);
