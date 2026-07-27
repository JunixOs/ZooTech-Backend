using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;

public interface IGetFecundacionForEditInputPort
    : IRequestHandler<GetFecundacionForEditQuery , GetFecundacionForEditOutput>
{
}
