using ZooTech.API.Models;

namespace ZooTech.API.Services;

public interface IVacunoRepository
{
    PagedResponse<VacunoResponse> Search(string? search, int page, int pageSize);
    VacunoResponse? GetByCodigo(string codigo);
    VacunoResponse? Update(string codigo, UpdateVacunoRequest request);
    bool Delete(string codigo);
    VacunoOptionsResponse GetOptions();
}

public sealed class InMemoryVacunoRepository : IVacunoRepository
{
    private readonly object syncRoot = new();
    private readonly List<VacunoResponse> vacunos = SeedVacunos();

    public PagedResponse<VacunoResponse> Search(string? search, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        lock (syncRoot)
        {
            var query = vacunos
                .Where(v => !string.Equals(v.Estado, "eliminado", StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(v =>
                    Contains(v.Codigo, term) ||
                    Contains(v.Nombre, term) ||
                    Contains(v.Raza, term) ||
                    Contains(v.Color, term) ||
                    Contains(v.Sexo, term) ||
                    Contains(v.Granja, term) ||
                    Contains(v.Distrito, term) ||
                    Contains(v.Departamento, term) ||
                    Contains(v.Provincia, term) ||
                    Contains(v.AptoPara, term) ||
                    Contains(v.Estado, term));
            }

            var filtered = query
                .OrderBy(v => v.Codigo, StringComparer.OrdinalIgnoreCase)
                .ToList();
            var totalItems = filtered.Count;
            var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
            var items = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResponse<VacunoResponse>(
                Items: items,
                Page: page,
                PageSize: pageSize,
                TotalItems: totalItems,
                TotalPages: totalPages);
        }
    }

    public VacunoResponse? GetByCodigo(string codigo)
    {
        lock (syncRoot)
        {
            return vacunos.FirstOrDefault(v =>
                string.Equals(v.Codigo, codigo, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(v.Estado, "eliminado", StringComparison.OrdinalIgnoreCase));
        }
    }

    public VacunoResponse? Update(string codigo, UpdateVacunoRequest request)
    {
        lock (syncRoot)
        {
            var index = vacunos.FindIndex(v =>
                string.Equals(v.Codigo, codigo, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(v.Estado, "eliminado", StringComparison.OrdinalIgnoreCase));

            if (index < 0)
            {
                return null;
            }

            var updated = new VacunoResponse(
                Codigo: vacunos[index].Codigo,
                Nombre: request.Nombre.Trim(),
                FechaNacimiento: request.FechaNacimiento,
                AdquisicionPor: request.AdquisicionPor,
                Raza: request.Raza,
                Color: request.Color,
                Sexo: request.Sexo,
                CodigoPadre: request.CodigoPadre,
                CodigoMadre: request.CodigoMadre,
                Granja: request.Granja,
                Distrito: request.Distrito,
                Departamento: request.Departamento,
                Provincia: request.Provincia,
                AptoPara: request.AptoPara,
                FechaRegistroFuncion: request.FechaRegistroFuncion,
                Observaciones: request.Observaciones,
                FotoUrl: request.FotoUrl,
                Estado: request.Estado);

            vacunos[index] = updated;
            return updated;
        }
    }

    public bool Delete(string codigo)
    {
        lock (syncRoot)
        {
            var index = vacunos.FindIndex(v =>
                string.Equals(v.Codigo, codigo, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(v.Estado, "eliminado", StringComparison.OrdinalIgnoreCase));

            if (index < 0)
            {
                return false;
            }

            vacunos[index] = vacunos[index] with { Estado = "eliminado" };
            return true;
        }
    }

    public VacunoOptionsResponse GetOptions()
    {
        lock (syncRoot)
        {
            return new VacunoOptionsResponse(
                AdquisicionOptions: vacunos.Select(v => v.AdquisicionPor).Distinct().Order().ToList(),
                RazaOptions: vacunos.Select(v => v.Raza).Distinct().Order().ToList(),
                SexoOptions: new[] { "Hembra", "Macho" },
                AptoParaOptions: vacunos.Select(v => v.AptoPara).Distinct().Order().ToList());
        }
    }

    private static bool Contains(string value, string term)
    {
        return value.Contains(term, StringComparison.OrdinalIgnoreCase);
    }

    private static List<VacunoResponse> SeedVacunos()
    {
        return
        [
            new("VAC_101", "Duquesa", "2023-02-01", "Inseminacion", "Holstein", "Marron", "Hembra", "VAC_0021", "VAC_0147", "Granjas Trejo", "Rupa Rupa", "Huanuco", "Leoncio Prado", "Produccion de leche", "2023-02-08", "Aqui pueden ir algunas observaciones sobre el vacuno en cuestion.", "https://images.unsplash.com/photo-1500595046743-cd271d694d30?auto=format&fit=crop&w=700&q=80", "activo"),
            new("VAC_102", "Luna", "2022-09-14", "Compra", "Brown Swiss", "Cafe claro", "Hembra", "VAC_0008", "VAC_0074", "Estancia El Prado", "Jose Crespo y Castillo", "Huanuco", "Leoncio Prado", "Produccion de leche", "2022-09-21", "Vacuno con buen rendimiento productivo y manejo regular.", "https://images.unsplash.com/photo-1527153857715-3908f2bae5e8?auto=format&fit=crop&w=700&q=80", "activo"),
            new("VAC_103", "Relampago", "2021-11-23", "Nacimiento", "Gyr", "Blanco con gris", "Macho", "VAC_0012", "VAC_0043", "Fundo Santa Elena", "Castillo Grande", "Huanuco", "Leoncio Prado", "Reproduccion", "2021-12-02", "Ejemplar destinado a reproduccion por historial genetico.", "https://images.unsplash.com/photo-1570042225831-d98fa7577f1e?auto=format&fit=crop&w=700&q=80", "activo"),
            new("VAC_104", "Canela", "2024-01-06", "Compra", "Jersey", "Dorado", "Hembra", "VAC_0031", "VAC_0099", "Granja Las Palmas", "Daniel Alomia Robles", "Huanuco", "Leoncio Prado", "Produccion de leche", "2024-01-18", "Se mantiene en control sanitario preventivo.", "https://images.unsplash.com/photo-1551298457-c72eced6d3e6?auto=format&fit=crop&w=700&q=80", "activo"),
            new("VAC_105", "Estrella", "2020-07-30", "Inseminacion", "Holstein", "Negro con blanco", "Hembra", "VAC_0015", "VAC_0038", "Granjas Trejo", "Rupa Rupa", "Huanuco", "Leoncio Prado", "Doble proposito", "2020-08-12", "Registro completo y sin incidencias recientes.", "https://images.unsplash.com/photo-1561043394-9f7d16d9ae37?auto=format&fit=crop&w=700&q=80", "activo"),
            new("VAC_106", "Rabuca", "2023-06-29", "Compra", "Simmental", "Marron claro", "Hembra", "VAC_0035", "VAC_0116", "Fundo Santa Elena", "Puerto Sungaro", "Huanuco", "Puerto Inca", "Doble proposito", "2023-07-03", "Adaptacion favorable al lote actual.", "https://images.unsplash.com/photo-1596733430284-f7437764b1a9?auto=format&fit=crop&w=700&q=80", "activo"),
            new("VAC_107", "Salada", "2023-07-10", "Nacimiento", "Simmental", "Marron", "Hembra", "VAC_0048", "VAC_0120", "Granja Las Palmas", "Tulumayo", "Huanuco", "Leoncio Prado", "Produccion de leche", "2023-07-17", "Sin observaciones criticas.", "https://images.unsplash.com/photo-1516467508483-a7212febe31a?auto=format&fit=crop&w=700&q=80", "activo"),
            new("VAC_108", "Viola", "2023-08-20", "Donacion", "Holstein", "Negro con blanco", "Hembra", "VAC_0041", "VAC_0141", "Estancia El Prado", "Tulumayo", "Huanuco", "Leoncio Prado", "Produccion de leche", "2023-08-28", "Requiere seguimiento de peso mensual.", "https://images.unsplash.com/photo-1594761051656-153faa7468c8?auto=format&fit=crop&w=700&q=80", "activo"),
            new("VAC_109", "Xata", "2023-09-27", "Compra", "Jersey", "Cafe", "Hembra", "VAC_0054", "VAC_0155", "Fundo Santa Elena", "Puerto Sungaro", "Huanuco", "Puerto Inca", "Engorde", "2023-10-01", "Ingreso validado con ficha completa.", "https://images.unsplash.com/photo-1535435734705-4f0f32e27c83?auto=format&fit=crop&w=700&q=80", "activo"),
            new("VAC_110", "Zamora", "2023-10-14", "Nacimiento", "Holstein", "Blanco con negro", "Hembra", "VAC_0061", "VAC_0162", "Granjas Trejo", "Tocache", "San Martin", "Tocache", "Produccion de leche", "2023-10-20", "Buen estado corporal en ultima revision.", "https://images.unsplash.com/photo-1545468800-85cc9bc6ecf7?auto=format&fit=crop&w=700&q=80", "activo"),
            new("VAC_111", "Nube", "2024-03-05", "Nacimiento", "Normando", "Gris", "Macho", "VAC_0065", "VAC_0168", "Granja Las Palmas", "Aucayacu", "Huanuco", "Leoncio Prado", "Engorde", "2024-03-12", "Registro de nacimiento validado por encargado.", "https://images.unsplash.com/photo-1529461850661-6e140a64a067?auto=format&fit=crop&w=700&q=80", "activo"),
            new("VAC_112", "Mora", "2021-05-19", "Compra", "Brown Swiss", "Cafe oscuro", "Hembra", "VAC_0072", "VAC_0181", "Estancia El Prado", "Rupa Rupa", "Huanuco", "Leoncio Prado", "Doble proposito", "2021-05-25", "Apta para controles productivos del periodo.", "https://images.unsplash.com/photo-1574943320219-553eb213f72d?auto=format&fit=crop&w=700&q=80", "inactivo")
        ];
    }
}
