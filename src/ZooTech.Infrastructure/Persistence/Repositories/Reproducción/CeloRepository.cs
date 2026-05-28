using Microsoft.EntityFrameworkCore;
using ZooTech.Application.DTOs.Reproduccion;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.Infrastructure.Persistence.Context;

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
                .Where(c => dto.CaracteristicaCodes.Contains(c.code))
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

        public async Task<bool> RegistrarCeloAsync(RegistrarCeloDTO dto)
        {
            var vacunoExiste = await _context.vacunos
                .AnyAsync(v => v.id == dto.VacunoId);

            if (!vacunoExiste)
                return false;

            var nuevoCelo = new celo_registro
            {
                codigo = $"CLO_{DateTime.Now:HHmmss}",
                fecha_hora = dto.FechaHora,
                vacuno_id = dto.VacunoId,
                encargado_usuario_id = dto.EncargadoUsuarioId,
                observaciones = dto.Observaciones,
                estado_registro_code = "ACTIVO",
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            var caracteristicas = await _context.cat_caracteristica_celos
                .Where(c => dto.CaracteristicaCodes.Contains(c.code))
                .ToListAsync();

            foreach (var caracteristica in caracteristicas)
            {
                nuevoCelo.caracteristica_codes.Add(caracteristica);
            }

            await _context.celo_registros.AddAsync(nuevoCelo);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

