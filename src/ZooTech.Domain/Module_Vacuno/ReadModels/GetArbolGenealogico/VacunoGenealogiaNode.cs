using System;
using System.Collections.Generic;
using System.Text;
using ZooTech.Domain.Module_Vacuno.Entities;

namespace ZooTech.Domain.Module_Vacuno.ReadModels.GetArbolGenealogico;

public sealed record VacunoGenealogiaNode(Vacuno Vacuno, int Nivel, string? Procedencia);