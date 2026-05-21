using Microsoft.EntityFrameworkCore;
using ZooTech.API.Models;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using System.Linq;

namespace ZooTech.API.Services;

public sealed class EfVacunoRepository : IVacunoRepository
{
    private readonly GanaderiaDbContext _db;

    public EfVacunoRepository(GanaderiaDbContext db)
    {
        _db = db;
    }

    public PagedResponse<VacunoResponse> Search(string? search, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _db.vacunos.AsQueryable()
            .Where(v => v.deleted_at == null);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(v =>
                EF.Functions.Like(v.codigo, $"%{term}%") ||
                EF.Functions.Like(v.nombre, $"%{term}%") ||
                EF.Functions.Like(v.raza_code, $"%{term}%") ||
                EF.Functions.Like(v.color_code, $"%{term}%") ||
                EF.Functions.Like(v.sexo_code, $"%{term}%") ||
                EF.Functions.Like(v.granja.nombre, $"%{term}%"));
        }

        var totalItems = query.Count();
        var entities = query
            .Include(x => x.granja)
            .Include(x => x.padre)
            .Include(x => x.madre)
            .OrderBy(v => v.codigo)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var items = entities.Select(v => new VacunoResponse(
                Codigo: FormatCodigo(v.codigo),
                Nombre: v.nombre,
                FechaNacimiento: v.fecha_nacimiento.ToString("yyyy-MM-dd"),
                AdquisicionPor: v.tipo_adquisicion_code,
                Raza: v.raza_code,
                Color: v.color_code,
                Sexo: v.sexo_code,
                CodigoPadre: v.padre?.codigo ?? string.Empty,
                CodigoMadre: v.madre?.codigo ?? string.Empty,
                Granja: v.granja?.nombre ?? string.Empty,
                Distrito: v.granja?.distrito_codigoNavigation?.nombre ?? string.Empty,
                Departamento: v.granja?.distrito_codigoNavigation?.provincia_codigoNavigation?.departamento_codigoNavigation?.nombre ?? string.Empty,
                Provincia: v.granja?.distrito_codigoNavigation?.provincia_codigoNavigation?.nombre ?? string.Empty,
                AptoPara: string.Empty,
                FechaRegistroFuncion: v.fecha_registro.ToString("yyyy-MM-dd"),
                Observaciones: v.observaciones ?? string.Empty,
                FotoUrl: string.Empty,
                Estado: v.deleted_at == null ? "activo" : "eliminado"
            ))
            .ToList();

        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PagedResponse<VacunoResponse>(items, page, pageSize, totalItems, totalPages);
    }

    public VacunoResponse? GetByCodigo(string codigo)
    {
        var searchKey = codigo.Replace("_", "");
        var v = _db.vacunos
            .Include(x => x.granja)
            .Include(x => x.padre)
            .Include(x => x.madre)
            .FirstOrDefault(x => (x.codigo == codigo || x.codigo.Replace("_", "") == searchKey) && x.deleted_at == null);

        if (v == null) return null;

        return new VacunoResponse(
            Codigo: FormatCodigo(v.codigo),
            Nombre: v.nombre,
            FechaNacimiento: v.fecha_nacimiento.ToString("yyyy-MM-dd"),
            AdquisicionPor: v.tipo_adquisicion_code,
            Raza: v.raza_code,
            Color: v.color_code,
            Sexo: v.sexo_code,
            CodigoPadre: v.padre?.codigo ?? string.Empty,
            CodigoMadre: v.madre?.codigo ?? string.Empty,
            Granja: v.granja.nombre,
            Distrito: v.granja.distrito_codigoNavigation?.nombre ?? string.Empty,
            Departamento: v.granja.distrito_codigoNavigation?.provincia_codigoNavigation?.departamento_codigoNavigation?.nombre ?? string.Empty,
            Provincia: v.granja.distrito_codigoNavigation?.provincia_codigoNavigation?.nombre ?? string.Empty,
            AptoPara: string.Empty,
            FechaRegistroFuncion: v.fecha_registro.ToString("yyyy-MM-dd"),
            Observaciones: v.observaciones ?? string.Empty,
            FotoUrl: string.Empty,
            Estado: v.deleted_at == null ? "activo" : "eliminado"
        );
    }

    private static string FormatCodigo(string codigo)
    {
        if (codigo.Contains('_')) return codigo;
        if (codigo.Length > 3)
        {
            return codigo.Insert(3, "_");
        }
        return codigo;
    }

    public VacunoResponse? Update(string codigo, UpdateVacunoRequest request)
    {
        var v = _db.vacunos.FirstOrDefault(x => x.codigo == codigo && x.deleted_at == null);
        if (v == null) return null;

        v.nombre = request.Nombre.Trim();
        v.observaciones = request.Observaciones;
        v.updated_at = DateTime.UtcNow;

        _db.SaveChanges();

        return GetByCodigo(codigo);
    }

    public bool Delete(string codigo)
    {
        var v = _db.vacunos.FirstOrDefault(x => x.codigo == codigo && x.deleted_at == null);
        if (v == null) return false;

        v.deleted_at = DateTime.UtcNow;
        v.motivo_eliminacion = "Eliminado por prueba";
        _db.SaveChanges();
        return true;
    }

    public VacunoOptionsResponse GetOptions()
    {
        var adquis = _db.vacunos.Select(v => v.tipo_adquisicion_code).Distinct().ToList();
        var razas = _db.vacunos.Select(v => v.raza_code).Distinct().ToList();
        var apto = new List<string>();

        return new VacunoOptionsResponse(adquis, razas, new List<string> { "Hembra", "Macho" }, apto);
    }
}
