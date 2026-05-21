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
}