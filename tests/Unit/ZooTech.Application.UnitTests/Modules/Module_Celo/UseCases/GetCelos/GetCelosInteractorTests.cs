using NSubstitute;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.GetCelos;

public sealed class GetCelosInteractorTests
{
    private readonly ICeloRepository _repository;
    private readonly GetCelosInteractor _interactor;

    public GetCelosInteractorTests()
    {
        _repository = Substitute.For<ICeloRepository>();
        _interactor = new GetCelosInteractor(_repository);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyItems_When_RepositoryIsEmpty()
    {
        _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<CeloListItem>());
        _repository.GetVecesEnCeloCountsAsync(Arg.Any<CancellationToken>()).Returns(new Dictionary<long, int>());

        var output = await _interactor.Handle(
            Application.Common.Models.EmptyCommand.Value(AuditEventType.Read, "Get celos"),
            CancellationToken.None);

        Assert.Empty(output.Items);
    }

    [Fact]
    public async Task Handle_Should_OrchestrateRepositoryCalls_And_PreserveOrderAndFields()
    {
        var celo1 = CeloListItem.Rehydrate(
            id: 1,
            codigo: "CEL-001",
            fechaHora: new DateTime(2026, 3, 1, 8, 0, 0),
            vacunoId: 10,
            vacunoCodigo: "VAC-010",
            nombreVacuno: "Manchada",
            observaciones: "Primera observación",
            caracteristicaCodes: ["FLUJO"]);
        var celo2 = CeloListItem.Rehydrate(
            id: 2,
            codigo: "CEL-002",
            fechaHora: new DateTime(2026, 3, 2, 9, 0, 0),
            vacunoId: 20,
            vacunoCodigo: "VAC-020",
            nombreVacuno: "Overa",
            observaciones: null,
            caracteristicaCodes: []);

        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<CeloListItem> { celo1, celo2 });
        _repository.GetVecesEnCeloCountsAsync(Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, int> { [10] = 4 });

        var output = await _interactor.Handle(
            Application.Common.Models.EmptyCommand.Value(AuditEventType.Read, "Get celos"),
            CancellationToken.None);

        Assert.Equal(2, output.Items.Count);
        Assert.Equal("CEL-001", output.Items[0].CodigoRegistro);
        Assert.Equal("CEL-002", output.Items[1].CodigoRegistro);
        Assert.Equal("Primera observación", output.Items[0].Observaciones);
        Assert.Null(output.Items[1].Observaciones);
        Assert.Equal(["FLUJO"], output.Items[0].CaracteristicaCodes);
        Assert.Empty(output.Items[1].CaracteristicaCodes);

        await _repository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        await _repository.Received(1).GetVecesEnCeloCountsAsync(Arg.Any<CancellationToken>());
    }
}
