using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Requests;
    public sealed record ListOrdeniosRequest(
    string Codigo,
    DateTime FechaHora,
    long VacunoId,
    long EncargadoUsuarioId,
    string NombreEncargado,
    decimal Litros,
    string EstadoOrdenioCode,
    string? Observaciones,
    string PageNumber,
    string PageSize
     
);


