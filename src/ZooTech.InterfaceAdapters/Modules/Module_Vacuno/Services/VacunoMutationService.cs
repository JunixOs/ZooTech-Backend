using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Services;

public sealed class VacunoMutationService : IVacunoMutationService
{
    private readonly GanaderiaDbContext _db;
    private readonly IVacunoReferenceResolver _referenceResolver;
    private readonly ICreateVacunoInputPort _createInputPort;
    private readonly IUpdateVacunoInputPort _updateInputPort;
    private readonly IVacunoResponseEnricher _responseEnricher;

    public VacunoMutationService(
        GanaderiaDbContext db,
        IVacunoReferenceResolver referenceResolver,
        ICreateVacunoInputPort createInputPort,
        IUpdateVacunoInputPort updateInputPort,
        IVacunoResponseEnricher responseEnricher)
    {
        _db = db;
        _referenceResolver = referenceResolver;
        _createInputPort = createInputPort;
        _updateInputPort = updateInputPort;
        _responseEnricher = responseEnricher;
    }

    public async Task<VacunoMutationResult> CreateAsync(
        CreateVacunoRequest request,
        CancellationToken cancellationToken = default)
    {
        var references = await _referenceResolver.ResolveForCreateAsync(request, cancellationToken);
        if (!references.Success)
        {
            return VacunoMutationResult.Fail(references.Error!);
        }

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        var granjaId = await EnsureGranjaIdAsync(references, cancellationToken);
        var command = VacunoMapper.ToCommand(request, references.PadreId, references.MadreId, granjaId);
        var output = await _createInputPort.HandleAsync(command, cancellationToken);
        var response = await _responseEnricher.EnrichAsync(output.Data, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return VacunoMutationResult.Ok(response);
    }

    public async Task<VacunoMutationResult> UpdateAsync(
        long id,
        UpdateVacunoRequest request,
        CancellationToken cancellationToken = default)
    {
        var references = await _referenceResolver.ResolveForUpdateAsync(id, request, cancellationToken);
        if (!references.Success)
        {
            return VacunoMutationResult.Fail(references.Error!);
        }

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        var granjaId = await EnsureGranjaIdAsync(references, cancellationToken);
        var command = VacunoMapper.ToCommand(request, references.PadreId, references.MadreId, granjaId);
        var output = await _updateInputPort.HandleAsync(id, command, cancellationToken);
        var response = await _responseEnricher.EnrichAsync(output.Data, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return VacunoMutationResult.Ok(response);
    }

    private async Task<long> EnsureGranjaIdAsync(
        VacunoReferenceResolution references,
        CancellationToken cancellationToken)
    {
        if (references.GranjaId.HasValue)
        {
            return references.GranjaId.Value;
        }

        var granjaToCreate = references.GranjaToCreate
            ?? throw new InvalidOperationException("No se pudo resolver la granja del vacuno.");

        var granjaExistente = await _db.granjas
            .Where(g => g.nombre == granjaToCreate.Nombre && g.distrito_codigo == granjaToCreate.CodigoDistrito)
            .Select(g => (long?)g.id)
            .FirstOrDefaultAsync(cancellationToken);

        if (granjaExistente.HasValue)
        {
            return granjaExistente.Value;
        }

        var now = DateTime.UtcNow;
        var granja = new ZooTech.Infrastructure.Persistence.Entities.granja
        {
            nombre = granjaToCreate.Nombre,
            distrito_codigo = granjaToCreate.CodigoDistrito,
            activo = true,
            created_at = now,
            updated_at = now
        };

        _db.granjas.Add(granja);
        await _db.SaveChangesAsync(cancellationToken);

        return granja.id;
    }
}
