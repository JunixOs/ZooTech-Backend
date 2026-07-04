using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories.GanaderiaDb;
using ZooTech.Domain.Ganaderia.Entities;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Mappers.GanaderiaDb;

namespace ZooTech.Infrastructure.Persistence.Repositories.GanaderiaDb
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IGanaderiaDbContextFactory _ganaderiaDbContextFactory;

        public UsuarioRepository(
            IGanaderiaDbContextFactory ganaderiaDbContextFactory
        )
        {
            _ganaderiaDbContextFactory = ganaderiaDbContextFactory;
        }


        public async Task Create(UsuarioDomainEntity usuarioDomainEntity)
        {
            var ganaderiaDbContext = await _ganaderiaDbContextFactory.CreateDbContextByTenantContext();

            await ganaderiaDbContext.usuarios.AddAsync(
                UsuarioMapper.toOrm(usuarioDomainEntity)
            );

            await ganaderiaDbContext.SaveChangesAsync();
        }

        public async Task<UsuarioDomainEntity?> GetByEmail(string email)
        {
            var ganaderiaDbContext = await _ganaderiaDbContextFactory.CreateDbContextByTenantContext();

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