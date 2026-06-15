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
            join raza in context.cat_razas.AsNoTracking()
                on animal.raza_code equals raza.code into razaJoin
            from raza in razaJoin.DefaultIfEmpty()
            join sexo in context.cat_sexos.AsNoTracking()
                on animal.sexo_code equals sexo.code into sexoJoin
            from sexo in sexoJoin.DefaultIfEmpty()
            join tipoAdquisicion in context.cat_tipo_adquisicions.AsNoTracking()
                on animal.tipo_adquisicion_code equals tipoAdquisicion.code into tipoAdquisicionJoin
            from tipoAdquisicion in tipoAdquisicionJoin.DefaultIfEmpty()
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
                Codigo = animal.codigo ?? DefaultText,
                Nombre = animal.nombre ?? DefaultText,
                Raza = raza != null ? raza.nombre : DefaultText,
                Sexo = sexo != null ? sexo.nombre : DefaultText,
                Procedencia = tipoAdquisicion != null ? tipoAdquisicion.nombre : DefaultText,
                Estado = estado != null ? estado.nombre : DefaultText,
                animal.fecha_registro
            };

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var keyword = filter.Keyword.Trim();
            query = query.Where(animal =>
                animal.Codigo.Contains(keyword) ||
                animal.Nombre.Contains(keyword));
        }

        return await query
            .OrderByDescending(animal => animal.fecha_registro)
            .ThenBy(animal => animal.Codigo)
            .Select(animal => new ReportAnimalListItem(
                animal.Codigo,
                animal.Nombre,
                animal.Raza,
                animal.Sexo,
                animal.Procedencia,
                animal.Estado,
                animal.fecha_registro))
            .ToArrayAsync(cancellationToken);
    }
}
