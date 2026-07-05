using System;
using System.Collections.Generic;
using System.Text;
using ZooTech.Domain.Module_Fecundacion.ReadModels;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;

public sealed record ListarFecundacionOutput(List<FecundacionListItem> Items, int TotalCount);
