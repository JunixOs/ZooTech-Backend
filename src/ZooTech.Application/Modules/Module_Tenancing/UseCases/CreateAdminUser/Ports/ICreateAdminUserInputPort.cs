using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser.Ports
{
    public interface ICreateAdminUserInputPort : IRequestHandler<CreateAdminUserCommand , CreateAdminUserOutput>
    {
    }
}