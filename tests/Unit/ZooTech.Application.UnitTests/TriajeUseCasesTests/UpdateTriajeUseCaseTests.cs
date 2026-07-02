using Moq;
using System;
using System.Threading.Tasks;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Sanidad.DTOs.Requests;
using ZooTech.Application.Modules.Module_Sanidad.UseCases;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using Xunit;

namespace ZooTech.Application.UnitTests;

public class UpdateTriajeUseCaseTests
{
    private readonly Mock<ITriajeRepository> _repositoryMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly UpdateTriajeUseCase _useCase;

    public UpdateTriajeUseCaseTests()
    {
        _repositoryMock = new Mock<ITriajeRepository>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _useCase = new UpdateTriajeUseCase(_repositoryMock.Object, _dateTimeProviderMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_TriajeExistente_DebeActualizarYRetornarTriaje()
    {
        // Arrange
        var id = 1L;
        var request = new TriajeRequest
        {
            VacunoId = 2,
            TipoPesoCode = "FINAL",
            PesoKg = 250.0m,
            Observaciones = "Actualizado",
            EstadoRegistroCode = "INACTIVO"
        };

        var existingTriaje = new Triaje
        {
            Id = id,
            Codigo = "TRI001",
            VacunoId = 1,
            TipoPesoCode = "CONTROL",
            PesoKg = 100.0m,
            Observaciones = "Normal",
            EstadoRegistroCode = "ACTIVO",
            CreatedAt = DateTime.Now.AddDays(-1)
        };

        var fakeCurrentTime = new DateTime(2026, 5, 20, 16, 0, 0);

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingTriaje);
        _dateTimeProviderMock.Setup(d => d.ServerNow).Returns(fakeCurrentTime);

        // Act
        var result = await _useCase.ExecuteAsync(id, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingTriaje.Codigo, result.Codigo); // El código no debe cambiar
        Assert.Equal(request.VacunoId, result.VacunoId);
        Assert.Equal(request.TipoPesoCode, result.TipoPesoCode);
        Assert.Equal(request.PesoKg, result.PesoKg);
        Assert.Equal(request.Observaciones, result.Observaciones);

        // Verifica que se haya llamado el UpdateAsync
        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Triaje>(t => 
            t.Id == id &&
            t.VacunoId == request.VacunoId &&
            t.PesoKg == request.PesoKg &&
            t.UpdatedAt == fakeCurrentTime
        )), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_TriajeNoExistente_DebeRetornarNull()
    {
        // Arrange
        var id = 99L;
        var request = new TriajeRequest();

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Triaje?)null);

        // Act
        var result = await _useCase.ExecuteAsync(id, request);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Triaje>()), Times.Never);
    }
}