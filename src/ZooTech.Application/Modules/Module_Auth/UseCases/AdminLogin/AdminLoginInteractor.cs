using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Identity;
using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Domain.Admin.Entities;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Auth.UseCases.AdminLogin
{
    public class AdminLoginInteractor : IAdminLoginInputPort, IAuditableRequest
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        private readonly IAdminUserRepository _adminUserRepository;
        

        public AuditEventType EventType => AuditEventType.Login;
        public string Action => "A user is login into the application.";

        public AdminLoginInteractor(
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            
            IAdminUserRepository adminUserRepository
        )
        {
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;

            _adminUserRepository = adminUserRepository;
        }

        public async Task<string> Handle(AdminLoginCommand cmd)
        {
            AdminUserDomainEntity? domainEntity = await _adminUserRepository.GetByEmail(cmd.Email);

            if(domainEntity is null)
            {
                throw new NotFoundException(ScopeName.Application , ModuleName.Auth);
            }

            if(!_passwordHasher.Compare(cmd.Password, domainEntity.PasswordHash))
            {
                throw new InvalidCredentialsException(ScopeName.Application , ModuleName.Auth);
            }

            var token = _jwtService.GenerateToken(
                domainEntity.Id,
                domainEntity.UserName ?? string.Empty,
                domainEntity.Email.Value,
                UserRole.Admin
            );

            domainEntity.UpdateLastLogin();
            await _adminUserRepository.Update(domainEntity);

            return token;
        }
    }
}