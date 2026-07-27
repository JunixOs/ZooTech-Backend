using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.GetFecundacionEstado;

public interface IGetFecundacionEstadoInputPort
    : IRequestHandler<GetFecundacionEstadoQuery , GetFecundacionEstadoOutput>
{
}