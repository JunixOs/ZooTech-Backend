using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Repositories;

public sealed class AnimalRepository : IAnimalRepository
{
    private readonly GanaderiaDbContext context;

    public AnimalRepository(GanaderiaDbContext context)
    {
        this.context = context;
    }

    public async Task<AnimalDeleteCandidate?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await context.vacunos
            .AsNoTracking()
            .Where(animal => animal.id == id && animal.deleted_at == null)
            .Select(animal => new AnimalDeleteCandidate(
                animal.id,
                animal.codigo,
                animal.nombre,
                animal.fecha_nacimiento,
                animal.granja_id,
                animal.created_at,
                animal.deleted_at))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AnimalDeleteCandidate>> GetActiveDuplicatesAsync(
        AnimalDeleteCandidate animal,
        CancellationToken cancellationToken = default)
    {
        return await context.vacunos
            .AsNoTracking()
            .Where(candidate =>
                candidate.id != animal.Id &&
                candidate.deleted_at == null &&
                (candidate.codigo == animal.Codigo ||
                    (candidate.nombre == animal.Nombre &&
                     candidate.fecha_nacimiento == animal.FechaNacimiento &&
                     candidate.granja_id == animal.GranjaId)))
            .Select(candidate => new AnimalDeleteCandidate(
                candidate.id,
                candidate.codigo,
                candidate.nombre,
                candidate.fecha_nacimiento,
                candidate.granja_id,
                candidate.created_at,
                candidate.deleted_at))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> HasDependenciesAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return
            await context.triajes.AnyAsync(item => item.vacuno_id == id && item.deleted_at == null, cancellationToken) ||
            await context.incidente_vacunos.AnyAsync(item => item.vacuno_id == id, cancellationToken) ||
            await context.ordenios.AnyAsync(item => item.vacuno_id == id && item.deleted_at == null, cancellationToken) ||
            await context.celo_registros.AnyAsync(item => item.vacuno_id == id && item.deleted_at == null, cancellationToken) ||
            await context.fecundacions.AnyAsync(item => item.vacuno_receptor_id == id, cancellationToken) ||
            await context.fecundacion_cria.AnyAsync(item => item.vacuno_hijo_id == id, cancellationToken) ||
            await context.fecundacion_donantes.AnyAsync(item => item.vacuno_donante_id == id, cancellationToken) ||
            await context.periodo_sequia.AnyAsync(item => item.vacuno_id == id, cancellationToken) ||
            await context.produccion_leche_estandars.AnyAsync(item => item.vacuno_id == id, cancellationToken) ||
            await context.vacuno_adquisicions.AnyAsync(item => item.vacuno_id == id, cancellationToken) ||
            await context.vacuno_estado_fecundacion_historials.AnyAsync(item => item.vacuno_id == id && item.deleted_at == null, cancellationToken) ||
            await context.vacuno_estado_historials.AnyAsync(item => item.vacuno_id == id, cancellationToken) ||
            await context.vacuno_fotos.AnyAsync(item => item.vacuno_id == id, cancellationToken) ||
            await context.vacuno_utilizacion_historials.AnyAsync(item => item.vacuno_id == id, cancellationToken) ||
            await context.vacunos.AnyAsync(item => item.padre_id == id || item.madre_id == id, cancellationToken);
    }

    public async Task SoftDeleteAsync(
        long id,
        string motivoEliminacion,
        long? eliminadoPor,
        DateTime fechaEliminacion,
        CancellationToken cancellationToken = default)
    {
        var animal = await context.vacunos
            .FirstOrDefaultAsync(item => item.id == id && item.deleted_at == null, cancellationToken);

        if (animal is null)
        {
            return;
        }

        animal.deleted_at = fechaEliminacion;
        animal.deleted_by = eliminadoPor;
        animal.motivo_eliminacion = motivoEliminacion;
        animal.updated_at = fechaEliminacion;
        animal.updated_by = eliminadoPor;

        await context.SaveChangesAsync(cancellationToken);
    }
}
