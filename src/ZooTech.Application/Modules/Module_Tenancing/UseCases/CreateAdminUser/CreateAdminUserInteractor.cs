using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Identity;
using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser.Ports;
using ZooTech.Domain.Admin.Entities;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.ValueObjects;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser
{
    public class CreateAdminUserInteractor : ICreateAdminUserInputPort, IAuditableRequest
    {
        public AuditEventType EventType => AuditEventType.Create;

        public IAdminUserRepository _adminUserRepository;
        public IPasswordHasher _passwordHasher;

        public CreateAdminUserInteractor(
            IAdminUserRepository adminUserRepository,
            IPasswordHasher passwordHasher
        )
        {
            _adminUserRepository = adminUserRepository;
            _passwordHasher = passwordHasher;
        }

        public string Action => "Create admin user";

        public async Task<CreateAdminUserOutput> Handle(CreateAdminUserCommand cmd)
        {
            await _adminUserRepository.Create(
                AdminUserDomainEntity.CreateFromCreationRequest(
                    email: new Email(cmd.Email),
                    userName: cmd.UserName,
                    passwordHash: _passwordHasher.Hash(cmd.Password),
                    firstName: cmd.FirstName,
                    lastName: cmd.LastName,
                    isActive: cmd.IsActive,
                    lastLoginAt: null,
                    metadata: cmd.Metadata,
                    updatedAt: null,
                    createdAt: null,
                    deletedAt: null
                )
            );

            return new CreateAdminUserOutput
            {
                Email = cmd.Email,
                UserName = cmd.UserName,
                FirstName = cmd.FirstName,
            };
        }
    }
}