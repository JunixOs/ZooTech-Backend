using ZooTech.Application.Modules.Module_Animals.UseCases.ListAnimals;

namespace ZooTech.InterfaceAdapters.Modules.Module_Animals.Presenters;

public class ListAnimalsPresenter : IListAnimalsOutputPort
{
    public object? Response { get; private set; }
    public int StatusCode { get; private set; } = 200;

    public Task Ok(ListAnimalsOutput output)
    {
        Response = new
        {
            data = output.PagedData.Data.Select(a => new
            {
                id = a.Id,
                codigo = a.Codigo,
                fechaRegistro = a.FechaRegistro.ToString("yyyy-MM-dd"),
                nombre = a.Nombre,
                raza = a.Raza,
                procedencia = a.Procedencia,
                estado = a.Estado.ToString().ToLower()
            }),
            pagination = new
            {
                page = output.PagedData.Page,
                limit = output.PagedData.PageSize,
                total = output.PagedData.Total,
                totalPages = output.PagedData.TotalPages
            }
        };
        StatusCode = 200;
        return Task.CompletedTask;
    }

    public Task Error(string code, string message)
    {
        Response = new
        {
            error = new
            {
                code = code,
                message = message,
                details = new List<object>()
            }
        };
        StatusCode = code == "FEATURE_DISABLED" ? 403 : 400; // Map business error to HTTP code
        return Task.CompletedTask;
    }
}
