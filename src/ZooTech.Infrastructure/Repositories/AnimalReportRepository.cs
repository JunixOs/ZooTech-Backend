using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Repositories;

public sealed class AnimalReportRepository : IAnimalReportRepository
{
    private const string DefaultText = "No especificado";
    private readonly GanaderiaDbContext context;

    public AnimalReportRepository(GanaderiaDbContext context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyCollection<ReportAnimalListItem>> GetAnimalListAsync(
        ReportAnimalListFilter filter,
        CancellationToken cancellationToken = default)
    {
        var query =
            from animal in context.vacunos.AsNoTracking()
            join estadoVigente in context.v_vacuno_estado_vigentes.AsNoTracking()
                on animal.id equals estadoVigente.vacuno_id into estadoVigenteJoin
            from estadoVigente in estadoVigenteJoin.DefaultIfEmpty()
            join estado in context.cat_estado_vacunos.AsNoTracking()
                on estadoVigente.estado_code equals estado.code into estadoJoin
            from estado in estadoJoin.DefaultIfEmpty()
            where animal.deleted_at == null &&
                  animal.fecha_registro >= filter.FechaInicio &&
                  animal.fecha_registro <= filter.FechaFin
            select new
            {
                animal.codigo,
                animal.nombre,
                Raza = animal.raza_codeNavigation.nombre,
                Sexo = animal.sexo_codeNavigation.nombre,
                Procedencia = animal.tipo_adquisicion_codeNavigation.nombre,
                Estado = estado != null ? estado.nombre : DefaultText,
                animal.fecha_registro
            };

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var keyword = filter.Keyword.Trim();
            query = query.Where(animal =>
                animal.codigo.Contains(keyword) ||
                animal.nombre.Contains(keyword));
        }

        return await query
            .OrderByDescending(animal => animal.fecha_registro)
            .ThenBy(animal => animal.codigo)
            .Select(animal => new ReportAnimalListItem(
                animal.codigo,
                animal.nombre,
                animal.Raza,
                animal.Sexo,
                animal.Procedencia,
                animal.Estado,
                animal.fecha_registro))
            .ToArrayAsync(cancellationToken);
    }
}
