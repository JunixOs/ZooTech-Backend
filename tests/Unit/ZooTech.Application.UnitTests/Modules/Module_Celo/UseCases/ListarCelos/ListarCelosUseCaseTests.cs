using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListarCelos;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.ListarCelos;

public class FakeCeloRepository : ICeloRepository
{
    public List<CeloListItemDto> Items { get; set; } = new();
    public Exception? ExceptionToThrow { get; set; }

    public Task<List<CeloListItemDto>> ListarCelosAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<List<CeloListItemDto>>(cancellationToken);
        }

        if (ExceptionToThrow != null)
        {
            return Task.FromException<List<CeloListItemDto>>(ExceptionToThrow);
        }

        return Task.FromResult(Items);
    }
}

public class ListarCelosUseCaseTests
{
    // --- PRUEBAS POSITIVAS ---

    [Fact]
    public async Task ExecuteAsync_ShouldReturnCelos_WhenRecordsExist()
    {
        // Arrange
        var fakeRepository = new FakeCeloRepository();
        var expectedItems = new List<CeloListItemDto>
        {
            new()
            {
                CodigoRegistro = "CR-001",
                Fecha = new DateOnly(2026, 5, 20),
                Hora = new TimeOnly(14, 30),
                CodigoVacuno = "V-123",
                NombreVacuno = "Blanca",
                VecesEnCelo = 2
            }
        };
        fakeRepository.Items = expectedItems;

        var useCase = new ListarCelosUseCase(fakeRepository);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("CR-001", result[0].CodigoRegistro);
        Assert.Equal("V-123", result[0].CodigoVacuno);
        Assert.Equal("Blanca", result[0].NombreVacuno);
        Assert.Equal(2, result[0].VecesEnCelo);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoRecordsExist()
    {
        // Arrange
        var fakeRepository = new FakeCeloRepository();
        fakeRepository.Items = new List<CeloListItemDto>();

        var useCase = new ListarCelosUseCase(fakeRepository);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // --- PRUEBAS NEGATIVAS ---

    [Fact]
    public async Task ExecuteAsync_ShouldPropagateException_WhenRepositoryThrowsException()
    {
        // Arrange
        var fakeRepository = new FakeCeloRepository();
        fakeRepository.ExceptionToThrow = new Exception("Database connection failure");

        var useCase = new ListarCelosUseCase(fakeRepository);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => useCase.ExecuteAsync());
        Assert.Equal("Database connection failure", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowOperationCanceledException_WhenCancellationTokenIsCancelled()
    {
        // Arrange
        var fakeRepository = new FakeCeloRepository();
        var useCase = new ListarCelosUseCase(fakeRepository);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync(); // Cancelamos el token de entrada

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => useCase.ExecuteAsync(cts.Token));
    }
}
