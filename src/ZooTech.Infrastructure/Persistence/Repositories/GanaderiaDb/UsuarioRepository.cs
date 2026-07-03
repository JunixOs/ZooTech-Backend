using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories.GanaderiaDb;
using ZooTech.Domain.Ganaderia.Entities;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Mappers.GanaderiaDb;

namespace ZooTech.Infrastructure.Persistence.Repositories.GanaderiaDb
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbContextFactory _dbContextFactory;

        public UsuarioRepository(
            IDbContextFactory dbContextFactory
        )
        {
            _dbContextFactory = dbContextFactory;
        }


        public async Task Create(UsuarioDomainEntity usuarioDomainEntity)
        {
            var ganaderiaDbContext = await _dbContextFactory.GetGanaderiaDbContext();

            await ganaderiaDbContext.usuarios.AddAsync(
                UsuarioMapper.toOrm(usuarioDomainEntity)
            );

            await ganaderiaDbContext.SaveChangesAsync();
        }

        public async Task<UsuarioDomainEntity?> GetByEmail(string email)
        {
            var ganaderiaDbContext = await _dbContextFactory.GetGanaderiaDbContext();

            var usuarioOrm = await ganaderiaDbContext.usuarios
                .Where(u => u.correo == email)
                .FirstOrDefaultAsync();
            
            if(usuarioOrm is null)
            {
                return null;
            }

            return UsuarioMapper.toDomain(usuarioOrm);
        }
    }
}