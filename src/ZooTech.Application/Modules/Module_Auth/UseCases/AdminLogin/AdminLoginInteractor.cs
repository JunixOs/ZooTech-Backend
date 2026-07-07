using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Identity;
using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Domain.Admin.Entities;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Auth.UseCases.AdminLogin
{
    public class AdminLoginInteractor : IAdminLoginInputPort
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        private readonly IAdminUserRepository _adminUserRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AdminLoginInteractor(
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            
            IAdminUserRepository adminUserRepository,
            IRefreshTokenRepository refreshTokenRepository
        )
        {
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;

            _adminUserRepository = adminUserRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<string> Handle(AdminLoginCommand cmd, CancellationToken cancellationToken = default)
        {
            AdminUserDomainEntity? domainEntity = await _adminUserRepository.GetByEmail(cmd.Email);

            if(domainEntity is null)
            {
                throw new NotFoundException(ScopeName.Application , ModuleName.Auth);
            }

            if (!domainEntity.IsActive)
            {
                throw new InactiveUserException(ScopeName.Application, ModuleName.Auth);
            }

            if(!_passwordHasher.Compare(cmd.Password, domainEntity.PasswordHash))
            {
                throw new InvalidCredentialsException(ScopeName.Application , ModuleName.Auth);
            }

            (var token, var jti) = _jwtService.GenerateToken(
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