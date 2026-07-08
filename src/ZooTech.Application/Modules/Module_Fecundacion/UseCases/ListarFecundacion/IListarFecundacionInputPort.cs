using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;

public interface IListarFecundacionInputPort
{
    Task<ListarFecundacionOutput> HandleAsync(
        ListarFecundacionCommand command, CancellationToken cancellationToken = default);
}