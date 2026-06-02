using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Vacunos.UseCases.GenerarArbolGenealogico;
using ZooTech.InterfaceAdapters.DTOs;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacunos.Presenters;

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
        Response = GeneralResponseDTO<object>.Fail(message);
        return Task.CompletedTask;
    }

    public Task Error(string code, string message)
    {
        StatusCode = 400; // O un StatusCode más específico según el code
        Response = GeneralResponseDTO<object>.Fail($"{code}: {message}");
        return Task.CompletedTask;
    }
}
