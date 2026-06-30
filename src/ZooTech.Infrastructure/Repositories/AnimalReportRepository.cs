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
            join color in context.cat_colors.AsNoTracking()
                on animal.color_code equals color.code into colorJoin
            from color in colorJoin.DefaultIfEmpty()
            join granja in context.granjas.AsNoTracking()
                on animal.granja_id equals granja.id into granjaJoin
            from granja in granjaJoin.DefaultIfEmpty()
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
                FechaNacimiento = animal.fecha_nacimiento,
                TipoAdquisicionCode = animal.tipo_adquisicion_code,
                TipoAdquisicion = tipoAdquisicion != null ? tipoAdquisicion.nombre : DefaultText,
                RazaCode = animal.raza_code,
                Raza = raza != null ? raza.nombre : DefaultText,
                ColorCode = animal.color_code,
                Color = color != null ? color.nombre : DefaultText,
                SexoCode = animal.sexo_code,
                Sexo = sexo != null ? sexo.nombre : DefaultText,
                GranjaId = animal.granja_id,
                Granja = granja != null ? granja.nombre : DefaultText,
                EstadoCode = estadoVigente != null ? estadoVigente.estado_code : null,
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

        if (!string.IsNullOrWhiteSpace(filter.RazaCode))
        {
            query = query.Where(animal => animal.RazaCode == filter.RazaCode);
        }
        if (!string.IsNullOrWhiteSpace(filter.ColorCode))
        {
            query = query.Where(animal => animal.ColorCode == filter.ColorCode);
        }

        if (!string.IsNullOrWhiteSpace(filter.SexoCode))
        {
            query = query.Where(animal => animal.SexoCode == filter.SexoCode);
        }

        if (!string.IsNullOrWhiteSpace(filter.TipoAdquisicionCode))
        {
            query = query.Where(animal => animal.TipoAdquisicionCode == filter.TipoAdquisicionCode);
        }

        if (filter.GranjaId.HasValue)
        {
            query = query.Where(animal => animal.GranjaId == filter.GranjaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.EstadoCode))
        {
            query = query.Where(animal => animal.EstadoCode == filter.EstadoCode);
        }

        return await query
            .OrderByDescending(animal => animal.fecha_registro)
            .ThenBy(animal => animal.Codigo)
            .Select(animal => new ReportAnimalListItem(
                animal.Codigo,
                animal.Nombre,
                animal.FechaNacimiento,
                animal.TipoAdquisicion,
                animal.Raza,
                animal.Color,
                animal.Sexo,
                animal.Granja,
                animal.Estado,
                animal.fecha_registro))
            .ToArrayAsync(cancellationToken);
    }
}
