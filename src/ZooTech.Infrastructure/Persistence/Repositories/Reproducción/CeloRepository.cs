using Microsoft.EntityFrameworkCore;
using ZooTech.Application.DTOs.Reproduccion;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Repositories.Reproduccion
{
    public class CeloRepository
    {
        private readonly GanaderiaDbContext _context;

        public CeloRepository(GanaderiaDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EditarCeloAsync(EditarCeloDTO dto)
        {
            var celo = await _context.celo_registros
                .Include(c => c.caracteristica_codes)
                .FirstOrDefaultAsync(c => c.id == dto.Id);

            if (celo == null)
                return false;

            // Actualizar observaciones
            celo.observaciones = dto.Observaciones;

            // Limpiar características actuales
            celo.caracteristica_codes.Clear();

            // Buscar nuevas características
            var caracteristicas = await _context.cat_caracteristica_celos
                .Where(c => dto.CaracteristicaIds.Contains(c.id))
                .ToListAsync();

            // Agregar nuevas características
            foreach (var caracteristica in caracteristicas)
            {
                celo.caracteristica_codes.Add(caracteristica);
            }

            celo.updated_at = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
