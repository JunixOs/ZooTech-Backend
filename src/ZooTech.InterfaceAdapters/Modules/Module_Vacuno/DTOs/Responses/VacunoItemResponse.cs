using System;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

public sealed record VacunoItemResponse(
    long Id,
    string Codigo,
    DateOnly FechaRegistro,
    string Nombre,
    string Raza,
    string? Procedencia,
    string Estado);
