using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;

namespace ZooTech.InterfaceAdapters.UnitTests.Modules.Module_Vacuno.Controllers;

public sealed class VacunoControllerDeleteTests
{
    [Fact]
    public async Task Delete_ShouldDelegateToOfficialDeleteAnimalInputPort()
    {
        var deleteAnimalInputPort = new CapturingDeleteAnimalInputPort();
        var controller = new VacunoController(
            listarInputPort: null!,
            createInputPort: null!,
            getByIdInputPort: null!,
            updateInputPort: null!,
            deleteAnimalInputPort: deleteAnimalInputPort)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
        controller.Request.Headers["X-User-Id"] = "34";

        var result = await controller.Delete(
            77,
            new DeleteVacunoRequest("Baja por duplicidad"),
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        Assert.Equal(1, deleteAnimalInputPort.Calls);
        Assert.NotNull(deleteAnimalInputPort.Command);
        Assert.Equal(77, deleteAnimalInputPort.Command.Id);
        Assert.Equal("Baja por duplicidad", deleteAnimalInputPort.Command.MotivoEliminacion);
        Assert.Equal(34, deleteAnimalInputPort.Command.EliminadoPor);
    }

    private sealed class CapturingDeleteAnimalInputPort : IDeleteAnimalInputPort
    {
        public int Calls { get; private set; }

        public DeleteAnimalCommand? Command { get; private set; }

        public Task Handle(
            DeleteAnimalCommand command,
            IDeleteAnimalOutputPort outputPort,
            CancellationToken cancellationToken = default)
        {
            Calls++;
            Command = command;
            outputPort.PresentSuccess(new DeleteAnimalOutput(
                command.Id,
                "VAC-001",
                "Luna",
                command.MotivoEliminacion!,
                command.EliminadoPor,
                new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                false));

            return Task.CompletedTask;
        }
    }
}
