using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Services;

public interface IVacunoMutationService
{
    Task<VacunoMutationResult> CreateAsync(
        CreateVacunoRequest request,
        CancellationToken cancellationToken = default);

    Task<VacunoMutationResult> UpdateAsync(
        long id,
        UpdateVacunoRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record VacunoMutationResult(
    VacunoResponse? Response,
    VacunoReferenceError? Error)
{
    public bool Success => Error is null;

    public static VacunoMutationResult Ok(VacunoResponse response) => new(response, null);

    public static VacunoMutationResult Fail(VacunoReferenceError error) => new(null, error);
}
