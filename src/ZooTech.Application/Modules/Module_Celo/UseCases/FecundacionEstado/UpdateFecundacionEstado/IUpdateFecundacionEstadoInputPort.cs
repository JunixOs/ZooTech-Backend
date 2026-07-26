using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;

public interface IUpdateFecundacionEstadoInputPort
    : IRequestHandler<UpdateFecundacionEstadoCommand , UpdateFecundacionEstadoOutput>
{
}