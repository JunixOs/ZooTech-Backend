using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;

namespace ZooTech.InterfaceAdapters.UnitTests.Modules.Module_Vacuno.Controllers;

public sealed class VacunoControllerDeleteTests
{
    [Fact]
    public async Task Delete_ShouldDelegateToOfficialDeleteVacunoInputPort()
    {
        var deleteVacunoInputPort = new CapturingDeleteVacunoInputPort();
        var controller = new VacunoController(
            getByIdInputPort: null!,
            getCatalogsInputPort: null!,
            deleteVacunoInputPort: deleteVacunoInputPort,
            vacunoRepository: null!,
            activityStatsReadRepository: null!,
            granjaReadRepository: null!,
            reporteUseCase: null!,
            listarVacunosReporteUseCase: null!,
            mutationService: null!,
            listarPaginadoInputPort: null!,
            listarPaginadoPresenter: null!,
            reportAnimalListInputPort: null!,
            exportarArbolInputPort: null!)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = await controller.Delete(
            77,
            new DeleteVacunoRequest("Baja por duplicidad"),
            CancellationToken.None);

        var noContent = Assert.IsType<NoContentResult>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContent.StatusCode);
        Assert.Equal(1, deleteVacunoInputPort.Calls);
        Assert.NotNull(deleteVacunoInputPort.Command);
        Assert.Equal(77, deleteVacunoInputPort.Id);
        Assert.Equal("Baja por duplicidad", deleteVacunoInputPort.Command.MotivoEliminacion);
    }

    private sealed class CapturingDeleteVacunoInputPort : IDeleteVacunoInputPort
    {
        public int Calls { get; private set; }
        public long Id { get; private set; }
        public DeleteVacunoCommand? Command { get; private set; }

        public Task HandleAsync(
            long id,
            DeleteVacunoCommand command,
            CancellationToken cancellationToken = default)
        {
            Calls++;
            Id = id;
            Command = command;
            return Task.CompletedTask;
        }
    }
}
