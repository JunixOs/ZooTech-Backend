using ZooTech.Application.Common.Gateway.Repositories.GanaderiaDb;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant.Ports;
using ZooTech.Domain.Ganaderia.Entities;
using ZooTech.Domain.Shared.ValueObjects;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant
{
    public class CreateUserInTenantInteractor : ICreateUserInTenantInputPort
    {
        private readonly IUsuarioRepository _usuarioRepository;        

        public CreateUserInTenantInteractor(
            IUsuarioRepository usuarioRepository
        )
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<CreateUserInTenantOutput> Handle(CreateUserInTenantCommand cmd, CancellationToken cancellationToken = default)
        {
            await _usuarioRepository.CreateInTenant(
                UsuarioDomainEntity.CreateFromRequest(
                    code: cmd.Code,
                    username: cmd.UserName,
                    fullName: cmd.FullName,
                    email: new Email(cmd.Email),
                    isActive: cmd.IsActive
                ),
                cmd.TenantDatabaseName
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