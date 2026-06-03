using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Infrastructure.Persistence;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno;

/// <summary>
/// Implementación concreta de IVacunoRepository usando EF Core.
/// El contexto ya está configurado para el tenant activo via TenantDbContextFactory.
/// </summary>
public sealed class VacunoRepository : IVacunoRepository
{
    private readonly GanaderiaDbContext _context;

    public VacunoRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<bool> ExisteCodigoAsync(string codigo, CancellationToken ct = default)
    {
        return await _context.Vacunos
            .AsNoTracking()
            .AnyAsync(v => v.Codigo == codigo.ToUpperInvariant(), ct);
    }

    /// <inheritdoc/>
    public async Task<Vacuno> RegistrarAsync(
        Vacuno vacuno,
        VacunoAdquisicion adquisicion,
        VacunoUtilizacionHistorial utilizacion,
        VacunoFoto? foto,
        CancellationToken ct = default)
    {
        // Usamos una transacción explícita para garantizar atomicidad
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            // 1. Insertar vacuno → EF asigna el ID generado por la BD
            await _context.Vacunos.AddAsync(vacuno, ct);
            await _context.SaveChangesAsync(ct);

            // 2. Asignar FK con el ID recién generado y persistir entidades relacionadas
            var idVacuno = vacuno.Id;

            var adquisicionConFk = VacunoAdquisicion.Crear(
                idVacuno: idVacuno,
                idTipoAdquisicion: adquisicion.IdTipoAdquisicion,
                precioCompra: adquisicion.PrecioCompra
            );

            var utilizacionConFk = VacunoUtilizacionHistorial.Crear(
                idVacuno: idVacuno,
                idTipoUtilizacion: utilizacion.IdTipoUtilizacion,
                aptoPara: utilizacion.AptoPara,
                fechaEspecificacion: utilizacion.FechaEspecificacion,
                observaciones: utilizacion.Observaciones
            );

            await _context.VacunosAdquisicion.AddAsync(adquisicionConFk, ct);
            await _context.VacunosUtilizacion.AddAsync(utilizacionConFk, ct);

            if (foto is not null)
            {
                var fotoConFk = VacunoFoto.Crear(idVacuno: idVacuno, rutaArchivo: foto.RutaArchivo);
                await _context.VacunosFoto.AddAsync(fotoConFk, ct);
            }

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return vacuno;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<IReadOnlyList<Vacuno>> ListarAsync(
        int page,
        int limit,
        string? q,
        string? estado,
        CancellationToken ct = default)
    {
        return await AplicarFiltros(q, estado)
            .Include(v => v.Adquisicion)
            .Include(v => v.Utilizacion)
            .Include(v => v.Foto)
            .OrderByDescending(v => v.CreadoEn)
            .Skip((page - 1) * limit)
            .Take(limit)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<int> ContarAsync(string? q, string? estado, CancellationToken ct = default)
    {
        return await AplicarFiltros(q, estado).CountAsync(ct);
    }

    public async Task<Vacuno?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Vacunos
            .Include(v => v.Adquisicion)
            .Include(v => v.Utilizacion)
            .Include(v => v.Foto)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id, ct);
    }

    public async Task<Vacuno?> ObtenerPorCodigoAsync(string codigo, CancellationToken ct = default)
    {
        var codigoNormalizado = codigo.Trim().ToUpperInvariant();

        return await _context.Vacunos
            .Include(v => v.Adquisicion)
            .Include(v => v.Utilizacion)
            .Include(v => v.Foto)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Codigo == codigoNormalizado, ct);
    }

    public async Task<bool> ExisteRegistroDuplicadoAsync(
        string codigoExcluido,
        EditarVacunoData data,
        CancellationToken ct = default)
    {
        var codigoNormalizado = codigoExcluido.Trim().ToUpperInvariant();

        return await _context.Vacunos
            .AsNoTracking()
            .AnyAsync(v =>
                v.Codigo != codigoNormalizado
                && v.Nombre == data.Nombre
                && v.IdRaza == data.IdRaza
                && v.IdColor == data.IdColor
                && v.IdSexo == data.IdSexo
                && v.IdGranja == data.IdGranja
                && v.IdDistrito == data.IdDistrito
                && v.IdDepartamento == data.IdDepartamento
                && v.IdProvincia == data.IdProvincia,
                ct);
    }

    public async Task<Vacuno?> ActualizarAsync(
        string codigo,
        EditarVacunoData data,
        CancellationToken ct = default)
    {
        var codigoNormalizado = codigo.Trim().ToUpperInvariant();

        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            var vacuno = await _context.Vacunos
                .Include(v => v.Adquisicion)
                .Include(v => v.Utilizacion)
                .Include(v => v.Foto)
                .FirstOrDefaultAsync(v => v.Codigo == codigoNormalizado, ct);

            if (vacuno is null)
                return null;

            vacuno.ActualizarRegistro(
                data.Nombre,
                data.IdRaza,
                data.Raza,
                data.IdColor,
                data.Color,
                data.IdSexo,
                data.Sexo,
                data.IdGranja,
                data.Granja,
                data.IdDistrito,
                data.Distrito,
                data.IdDepartamento,
                data.Departamento,
                data.IdProvincia,
                data.Provincia,
                data.ActualizadoEn);

            if (vacuno.Adquisicion is null)
            {
                await _context.VacunosAdquisicion.AddAsync(
                    VacunoAdquisicion.Crear(vacuno.Id, data.IdTipoAdquisicion, data.PrecioCompra),
                    ct);
            }
            else
            {
                vacuno.Adquisicion.Actualizar(data.IdTipoAdquisicion, data.PrecioCompra);
            }

            if (vacuno.Utilizacion is null)
            {
                await _context.VacunosUtilizacion.AddAsync(
                    VacunoUtilizacionHistorial.Crear(
                        vacuno.Id,
                        data.IdTipoUtilizacion,
                        data.AptoPara,
                        data.FechaEspecificacion,
                        data.Observaciones),
                    ct);
            }
            else
            {
                vacuno.Utilizacion.Actualizar(
                    data.IdTipoUtilizacion,
                    data.AptoPara,
                    data.FechaEspecificacion,
                    data.Observaciones);
            }

            if (!string.IsNullOrWhiteSpace(data.RutaFoto))
            {
                if (vacuno.Foto is null)
                {
                    await _context.VacunosFoto.AddAsync(VacunoFoto.Crear(vacuno.Id, data.RutaFoto), ct);
                }
                else
                {
                    vacuno.Foto.ActualizarRuta(data.RutaFoto);
                }
            }

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return await ObtenerPorCodigoAsync(codigoNormalizado, ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<IReadOnlyList<UbigeoOption>> ListarUbigeoAsync(CancellationToken ct = default)
    {
        try
        {
            var departamentos = await _context.GeoDepartamentos
                .AsNoTracking()
                .OrderBy(departamento => departamento.nombre)
                .Select(departamento => new
                {
                    departamento.codigo,
                    departamento.nombre
                })
                .ToListAsync(ct);

            var provincias = await _context.GeoProvincias
                .AsNoTracking()
                .OrderBy(provincia => provincia.nombre)
                .Select(provincia => new
                {
                    provincia.codigo,
                    provincia.nombre,
                    provincia.departamento_codigo
                })
                .ToListAsync(ct);

            var distritos = await _context.GeoDistritos
                .AsNoTracking()
                .OrderBy(distrito => distrito.nombre)
                .Select(distrito => new
                {
                    distrito.codigo,
                    distrito.nombre,
                    distrito.provincia_codigo
                })
                .ToListAsync(ct);

            return departamentos
                .Select(departamento => new UbigeoOption(
                    departamento.codigo,
                    departamento.nombre,
                    provincias
                        .Where(provincia => provincia.departamento_codigo == departamento.codigo)
                        .Select(provincia => new UbigeoOption(
                            provincia.codigo,
                            provincia.nombre,
                            distritos
                                .Where(distrito => distrito.provincia_codigo == provincia.codigo)
                                .Select(distrito => new UbigeoOption(distrito.codigo, distrito.nombre, []))
                                .ToList()))
                        .ToList()))
                .ToList();
        }
        catch (SqlException exception) when (exception.Number == 208)
        {
            return [];
        }
    }

    private IQueryable<Vacuno> AplicarFiltros(string? q, string? estado)
    {
        var query = _context.Vacunos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var texto = q.Trim().ToUpperInvariant();
            query = query.Where(v => v.Codigo.Contains(texto) || v.Nombre.Contains(q.Trim()));
        }

        if (string.Equals(estado, "vivo", StringComparison.OrdinalIgnoreCase))
            query = query.Where(v => v.IdEstado == 1);

        if (string.Equals(estado, "muerto", StringComparison.OrdinalIgnoreCase))
            query = query.Where(v => v.IdEstado != 1);

        return query;
    }
}
