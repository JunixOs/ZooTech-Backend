using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;

public sealed record ListOrdenioCommand(
    string Codigo,
    DateTime FechaHora,
    long VacunoId,
    long EncargadoUsuarioId,
    string EncargadoUsuarioNombre,
    decimal Litros,
    string EstadoOrdenioCode,
    string? Observaciones
);

