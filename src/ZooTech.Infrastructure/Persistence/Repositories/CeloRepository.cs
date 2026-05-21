using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListarCelos;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Repositories;

public class CeloRepository : ICeloRepository
{
    private readonly GanaderiaDbContext _context;

    public CeloRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public async Task<List<CeloListItemDto>> ListarCelosAsync(CancellationToken cancellationToken = default)
    {
        var resultado = await (
            from cr in _context.celo_registros
            join v in _context.vacunos on cr.vacuno_id equals v.id
            where cr.deleted_at == null
            orderby cr.fecha_hora descending
            select new CeloListItemDto
            {
                CodigoRegistro = cr.codigo,
                Fecha = DateOnly.FromDateTime(cr.fecha_hora),
                Hora = TimeOnly.FromDateTime(cr.fecha_hora),
                CodigoVacuno = v.codigo,
                NombreVacuno = v.nombre,
                VecesEnCelo = _context.celo_registros
                    .Count(x => x.vacuno_id == cr.vacuno_id && x.deleted_at == null)
            }
        ).ToListAsync(cancellationToken);

        return resultado;
    }
}
