using System;
using System.Collections.Generic;
using System.Text;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.Entities.GetArbolGenealogico;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;

public sealed record GetArbolGenealogicoOutput(List<GetArbolGenealogicoItem> Arbol);