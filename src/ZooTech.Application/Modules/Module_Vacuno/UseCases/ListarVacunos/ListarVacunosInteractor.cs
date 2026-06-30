using ZooTech.Domain.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public sealed class ListarVacunosInteractor : IListarVacunosInputPort
{
    private readonly IVacunoListadoReadRepository _repository;

    public ListarVacunosInteractor(IVacunoListadoReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<ListarVacunosOutput> HandleAsync(
        ListarVacunosQuery? query = null,
        CancellationToken cancellationToken = default)
    {
        var vacunos = await _repository.ListarAsync(
            new VacunoListadoReadQuery(
                Normalize(query?.Q),
                NormalizeEstado(query?.Estado),
                query?.FechaDesde,
                query?.FechaHasta),
            cancellationToken);

        var items = vacunos.Select(x => new VacunoItemDto(
            Id: x.Id,
            Codigo: x.Codigo,
            Nombre: x.Nombre,
            FechaNacimiento: x.FechaNacimiento,
            FechaRegistro: x.FechaRegistro,
            RazaCode: x.RazaCode,
            SexoCode: x.SexoCode,
            Procedencia: x.Procedencia,
            IsDeleted: x.IsDeleted)).ToList();

        return new ListarVacunosOutput(items);
    }

    private static string? Normalize(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    private static string? NormalizeEstado(string? value)
    {
        var normalized = Normalize(value)?.ToLowerInvariant();
        return normalized is "activo" or "eliminado" ? normalized : null;
    }
}
