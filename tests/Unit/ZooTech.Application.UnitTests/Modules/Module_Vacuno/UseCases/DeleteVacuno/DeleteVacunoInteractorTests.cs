using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases.DeleteVacuno;

public class DeleteVacunoInteractorTests
{
    private readonly IVacunoRepository _vacunoRepository = Substitute.For<IVacunoRepository>();
    private readonly ICeloRepository _celoRepository = Substitute.For<ICeloRepository>();
    private readonly ITriajeRepository _triajeRepository = Substitute.For<ITriajeRepository>();
    private readonly IOrdenioRepository _ordenioRepository = Substitute.For<IOrdenioRepository>();
    private readonly DeleteVacunoInteractor _interactor;

    public DeleteVacunoInteractorTests()
    {
        _interactor = new DeleteVacunoInteractor(
            _vacunoRepository,
            _celoRepository,
            _triajeRepository,
            _ordenioRepository);
    }

    private static Vacuno CreateTestVacuno()
    {
        return Vacuno.CreateNew(
            codigo: "V001",
            nombre: "Lola",
            fechaNacimiento: new DateOnly(2022, 1, 1),
            tipoAdquisicionCode: "NAC",
            razaCode: "HOL",
            colorCode: "BLA",
            sexoCode: "H",
            padreId: null,
            madreId: null,
            granjaId: 1,
            observaciones: "Sin obs",
            actorUsuarioId: 1,
            utcNow: DateTime.UtcNow);
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        _vacunoRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Vacuno?)null);
        var command = new DeleteVacunoCommand(1, "Motivo test");

        // Act & Assert
        await Assert.ThrowsAsync<VacunoNotFoundException>(() => _interactor.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoHasActiveCeloRecords_ShouldThrowVacunoHasDependenciesException()
    {
        // Arrange
        var vacuno = CreateTestVacuno();
        _vacunoRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(vacuno);
        _celoRepository.HasActiveRecordsByVacunoAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        var command = new DeleteVacunoCommand(1, "Motivo test");

        // Act & Assert
        var ex = await Assert.ThrowsAsync<VacunoHasDependenciesException>(() => _interactor.HandleAsync(command, CancellationToken.None));
        Assert.Contains("celo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoHasActiveOrdenioRecords_ShouldThrowVacunoHasDependenciesException()
    {
        // Arrange
        var vacuno = CreateTestVacuno();
        _vacunoRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(vacuno);
        _celoRepository.HasActiveRecordsByVacunoAsync(1, Arg.Any<CancellationToken>()).Returns(false);
        _ordenioRepository.HasActiveRecordsByVacunoAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        var command = new DeleteVacunoCommand(1, "Motivo test");

        // Act & Assert
        var ex = await Assert.ThrowsAsync<VacunoHasDependenciesException>(() => _interactor.HandleAsync(command, CancellationToken.None));
        Assert.Contains("ordeño", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoHasNoDependencies_ShouldSoftDeleteAndUpdate()
    {
        // Arrange
        var vacuno = CreateTestVacuno();
        _vacunoRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(vacuno);
        _celoRepository.HasActiveRecordsByVacunoAsync(1, Arg.Any<CancellationToken>()).Returns(false);
        _ordenioRepository.HasActiveRecordsByVacunoAsync(1, Arg.Any<CancellationToken>()).Returns(false);
        _triajeRepository.GetHistorialByVacunoIdAsync(1, null, null, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<TriajeHistorialItem>());
        _vacunoRepository.UpdateAsync(vacuno, null, null, Arg.Any<CancellationToken>()).Returns(vacuno);

        var command = new DeleteVacunoCommand(1, "Venta de vacuno");

        // Act
        var result = await _interactor.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal(EmptyOutput.Value, result);
        await _vacunoRepository.Received(1).UpdateAsync(vacuno, null, null, Arg.Any<CancellationToken>());
        Assert.NotNull(vacuno.DeletedAt);
        Assert.Equal("Venta de vacuno", vacuno.MotivoEliminacion);
    }
}
