using ZooTech.Domain.Ganaderia.Entities;

namespace ZooTech.Application.Common.Gateway.Repositories.GanaderiaDb
{
    public interface IUsuarioRepository
    {
        Task Create(UsuarioDomainEntity usuarioDomainEntity);
        
        Task<UsuarioDomainEntity?> GetByEmail(string email);
    }
}