using ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Presenters;

public class GenerarArbolGenealogicoPresenter : IGenerarArbolGenealogicoOutputPort
{
    public int StatusCode { get; private set; }
    public object Response { get; private set; } = default!;

    public Task Ok(GenerarArbolGenealogicoOutput output)
    {
        StatusCode = 200;
        Response = GeneralResponseDTO<GenerarArbolGenealogicoOutput>.Ok(output);
        return Task.CompletedTask;
    }

    public Task NotFound(string message)
    {
        StatusCode = 404;
        Response = ErrorResponse.Create("NOT_FOUND", message);
        return Task.CompletedTask;
    }

    public Task Error(string code, string message)
    {
        StatusCode = 400;
        Response = ErrorResponse.Create(code, message);
        return Task.CompletedTask;
    }
}
