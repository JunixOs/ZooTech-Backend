using System;
using System.Collections.Generic;
using System.Text;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Entities;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;

public sealed record ListarFecundacionOutput(List<FecundacionListItem> Items, int TotalCount);
