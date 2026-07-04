using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunosPaginado;
using ZooTech.InterfaceAdapters.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Presenters;

public class ListarVacunosPaginadoPresenter : IListarVacunosPaginadoOutputPort
{
    public object? Response { get; private set; }
    public int StatusCode { get; private set; } = 200;

    public Task Ok(ListarVacunosPaginadoOutput output)
    {
        Response = new
        {
            data = output.PagedData.Data.Select(v => new
            {
                id = v.Id,
                codigo = v.Codigo,
                fechaRegistro = v.FechaRegistro.ToString("yyyy-MM-dd"),
                nombre = v.Nombre,
                raza = v.Raza,
                procedencia = v.Procedencia,
                estado = v.Estado.ToString()
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
        Response = ErrorResponse.Create(code, message);
        StatusCode = code == "FEATURE_DISABLED" ? 403 : 400;
        return Task.CompletedTask;
    }
}
