using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Repositories.GanaderiaDb;
using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant.Ports;
using ZooTech.Domain.Ganaderia.Entities;
using ZooTech.Domain.Shared.ValueObjects;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant
{
    public class CreateUserInTenantInteractor : ICreateUserInTenantInputPort
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITenantRepository _tenantRepository;    

        public CreateUserInTenantInteractor(
            ITenantRepository tenantRepository,
            IUsuarioRepository usuarioRepository
        )
        {
            _usuarioRepository = usuarioRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<CreateUserInTenantOutput> HandleAsync(CreateUserInTenantCommand cmd, CancellationToken cancellationToken = default)
        {
            var tenantDatabaseName = await _tenantRepository.GetTenantDatabaseNameByTenantId(cmd.TenantId.GetValueOrDefault());

            if(tenantDatabaseName is null)
            {
                throw new NotFoundException(
                    Domain.Shared.Enums.ScopeName.Application , 
                    Domain.Shared.Enums.ModuleName.Tenancing , 
                    $"Tenant With ID [{cmd.TenantId}] Not Found"
                );
            }

            await _usuarioRepository.CreateInTenant(
                UsuarioDomainEntity.CreateFromRequest(
                    code: cmd.Code,
                    username: cmd.UserName,
                    fullName: cmd.FullName,
                    email: new Email(cmd.Email),
                    isActive: cmd.IsActive
                ),
                tenantDatabaseName
            );

            return new CreateUserInTenantOutput
            {
                Email = cmd.Email,
                UserName = cmd.UserName,
                FullName = cmd.FullName
            };
        }
    }
}