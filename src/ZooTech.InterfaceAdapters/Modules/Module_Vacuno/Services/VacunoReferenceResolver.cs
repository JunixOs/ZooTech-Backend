using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Application.Modules.Module_Vacuno.Services;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Services;

public sealed class VacunoReferenceResolver : IVacunoReferenceResolver
{
    private readonly IVacunoReferenceReadRepository _referenceReadRepository;

    public VacunoReferenceResolver(IVacunoReferenceReadRepository referenceReadRepository)
    {
        _referenceReadRepository = referenceReadRepository;
    }

    public async Task<VacunoReferenceResolution> ResolveAsync(
        VacunoReferenceData referenceData,
        CancellationToken cancellationToken = default)
    {
        var kinship = await ResolveKinshipAsync(
            referenceData.CodigoPadre,
            referenceData.CodigoMadre,
            referenceData.OwnCodigo,
            cancellationToken);

        if (kinship.Error is not null)
        {
            return VacunoReferenceResolution.Fail(kinship.Error);
        }

        var granja = await ResolveGranjaAsync(
            referenceData.GranjaId,
            referenceData.GranjaNombre,
            referenceData.CodigoDistrito,
            cancellationToken);

        return BuildResolution(kinship.PadreId, kinship.MadreId, granja);
    }

    private async Task<KinshipResolution> ResolveKinshipAsync(
        string? codigoPadre,
        string? codigoMadre,
        string ownCodigo,
        CancellationToken cancellationToken)
    {
        var padreCode = Normalize(codigoPadre);
        var madreCode = Normalize(codigoMadre);

        if (padreCode is not null && padreCode == ownCodigo)
        {
            return KinshipResolution.Fail("codigoPadre", "Un vacuno no puede ser su propio padre.");
        }

        if (madreCode is not null && madreCode == ownCodigo)
        {
            return KinshipResolution.Fail("codigoMadre", "Un vacuno no puede ser su propia madre.");
        }

        long? padreId = null;
        if (padreCode is not null)
        {
            var padre = await _referenceReadRepository.GetActiveByCodigoAsync(padreCode, cancellationToken);

            if (padre is null)
            {
                return KinshipResolution.Fail(
                    "codigoPadre",
                    "El vacuno padre especificado no existe.");
            }

            padreId = padre.Id;
        }

        long? madreId = null;
        if (madreCode is not null)
        {
            var madre = await _referenceReadRepository.GetActiveByCodigoAsync(madreCode, cancellationToken);

            if (madre is null)
            {
                return KinshipResolution.Fail(
                    "codigoMadre",
                    "El vacuno madre especificado no existe.");
            }

            madreId = madre.Id;
        }

        return KinshipResolution.Ok(padreId, madreId);
    }

    private async Task<GranjaResolution> ResolveGranjaAsync(
        long? granjaId,
        string? granjaNombre,
        string? codigoDistrito,
        CancellationToken cancellationToken)
    {
        if (granjaId.HasValue && granjaId.Value > 0)
        {
            var granjaExiste = await _referenceReadRepository.ExistsActiveGranjaAsync(
                granjaId.Value,
                cancellationToken);

            return granjaExiste
                ? GranjaResolution.Existing(granjaId.Value)
                : GranjaResolution.Fail(
                    "granjaId",
                    "La granja seleccionada no existe o no está activa.");
        }

        var nombre = Normalize(granjaNombre);
        var distrito = Normalize(codigoDistrito);
        if (nombre is null || distrito is null)
        {
            return GranjaResolution.Existing(0);
        }

        var distritoExists = await _referenceReadRepository.ExistsDistritoAsync(distrito, cancellationToken);

        if (!distritoExists)
        {
            return GranjaResolution.Fail(
                "codigoDistrito",
                "El distrito especificado no es válido o no está registrado.");
        }

        var granjaIdExistente = await _referenceReadRepository.FindGranjaIdAsync(
            nombre,
            distrito,
            cancellationToken);

        return granjaIdExistente.HasValue
            ? GranjaResolution.Existing(granjaIdExistente.Value)
            : GranjaResolution.New(nombre, distrito);
    }

    private static VacunoReferenceResolution BuildResolution(
        long? padreId,
        long? madreId,
        GranjaResolution granja)
    {
        if (granja.Error is not null)
        {
            return VacunoReferenceResolution.Fail(granja.Error);
        }

        return granja.GranjaToCreate is not null
            ? VacunoReferenceResolution.OkNewGranja(
                padreId,
                madreId,
                granja.GranjaToCreate.Nombre,
                granja.GranjaToCreate.CodigoDistrito)
            : VacunoReferenceResolution.Ok(padreId, madreId, granja.GranjaId);
    }

    private static string? Normalize(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    private sealed record KinshipResolution(long? PadreId, long? MadreId, VacunoReferenceError? Error)
    {
        public static KinshipResolution Ok(long? padreId, long? madreId) => new(padreId, madreId, null);

        public static KinshipResolution Fail(string field, string message)
            => new(null, null, new VacunoReferenceError(field, message, VacunoReferenceErrorKind.Validation));
    }

    private sealed record GranjaResolution(
        long GranjaId,
        VacunoGranjaCreation? GranjaToCreate,
        VacunoReferenceError? Error)
    {
        public static GranjaResolution Existing(long granjaId) => new(granjaId, null, null);

        public static GranjaResolution New(string nombre, string codigoDistrito)
            => new(0, new VacunoGranjaCreation(nombre, codigoDistrito), null);

        public static GranjaResolution Fail(string field, string message)
            => new(0, null, new VacunoReferenceError(field, message, VacunoReferenceErrorKind.Validation));
    }
}
