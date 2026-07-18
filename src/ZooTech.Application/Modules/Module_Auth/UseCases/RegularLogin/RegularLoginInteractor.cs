using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Identity;
using ZooTech.Application.Common.Gateway.Repositories.GanaderiaDb;
using ZooTech.Domain.Ganaderia.Entities;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Auth.UseCases.RegularLogin
{
    public class RegularLoginInteractor : IRegularLoginInputPort
    {
        private readonly IAppCacheService _appCacheService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IJwtService _jwtService;

        public RegularLoginInteractor(
            IAppCacheService appCacheService,
            IUsuarioRepository usuarioRepository,
            IJwtService jwtService
        )
        {
            _appCacheService = appCacheService;
            _usuarioRepository = usuarioRepository;
            _jwtService = jwtService;
        }

        public async Task<string> Handle(RegularLoginCommand cmd, CancellationToken cancellationToken = default)
        {
            UsuarioDomainEntity? domainEntity = await _usuarioRepository.GetByEmail(cmd.Email);
            
            if(domainEntity is null)
            {
                throw new NotFoundException(ScopeName.Application , ModuleName.Auth);
            }

            if (!domainEntity.IsActive)
            {
                throw new InactiveUserException(ScopeName.Application, ModuleName.Auth);
            }

            (var token, var jti) = _jwtService.GenerateToken(
                domainEntity.Id,
                domainEntity.UserName ?? string.Empty,
                domainEntity.Email.Value,
                UserRole.Regular
            );

            // Cantidad de minutos que durara la sesion
            await _appCacheService.SaveAsync(
                $"reg-user:{domainEntity.Id}",
                token,
                TimeSpan.FromMinutes(30)
            );

            return token;
        }
    }
}