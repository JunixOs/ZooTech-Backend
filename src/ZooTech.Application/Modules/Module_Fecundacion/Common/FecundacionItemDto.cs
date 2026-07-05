using System;

namespace ZooTech.Application.Modules.Module_Fecundacion.Common;

public sealed record FecundacionItemDto(
    long Id,
    string Codigo,
    DateOnly Fecha,
    string Vacuno,
    string TipoFecundacion,
    string ToroODonante,
    string Responsable,
    string Estado);
