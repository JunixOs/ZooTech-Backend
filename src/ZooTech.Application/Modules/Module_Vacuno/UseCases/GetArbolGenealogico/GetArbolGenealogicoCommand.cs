using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;

public record GetArbolGenealogicoCommand(long Id, int Niveles);