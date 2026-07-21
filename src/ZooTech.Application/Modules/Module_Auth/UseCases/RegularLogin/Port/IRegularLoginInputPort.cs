using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Auth.UseCases.RegularLogin
{
    public interface IRegularLoginInputPort : IRequestHandler<RegularLoginCommand , string>
    {
    }
}