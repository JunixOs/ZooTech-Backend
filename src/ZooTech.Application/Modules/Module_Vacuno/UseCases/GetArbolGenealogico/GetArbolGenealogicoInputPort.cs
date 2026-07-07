using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;
public interface IGetArbolGenealogicoInputPort
{
    Task<GetArbolGenealogicoOutput> HandleAsync(
        GetArbolGenealogicoCommand command, CancellationToken cancellationToken = default);
}