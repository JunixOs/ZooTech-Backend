using System;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacionesPaginado;

public sealed record ListarFecundacionesPaginadoCommand(
    int Page,
    int Limit,
    DateOnly? FechaDesde,
    DateOnly? FechaHasta,
    string? Q,
    string? TipoFecundacion,
    string? Estado,
    string? Responsable);
