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

    public Task<List<CeloListItemDto>> ListarCelosAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Items);
    }
}

public class ListarCelosUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnCelosFromRepository()
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
}
