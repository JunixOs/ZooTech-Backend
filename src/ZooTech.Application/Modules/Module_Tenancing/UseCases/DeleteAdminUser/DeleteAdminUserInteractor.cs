using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.DeleteAdminUser.Ports;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.DeleteAdminUser
{
    public class DeleteAdminUserInteractor : IDeleteAdminUserInputPort
    {
        public IAdminUserRepository _adminUserRepository;
        
        public DeleteAdminUserInteractor(
            IAdminUserRepository adminUserRepository
        )
        {
            _adminUserRepository = adminUserRepository;
        }

        public async Task<EmptyOutput> HandleAsync(DeleteAdminUserCommand cmd, CancellationToken cancellationToken = default)
        {
            await _adminUserRepository.DeleteById(cmd.Id.GetValueOrDefault());

            return EmptyOutput.Value;
        }
    }
}