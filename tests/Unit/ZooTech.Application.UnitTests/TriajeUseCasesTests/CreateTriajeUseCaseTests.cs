using Moq;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Sanidad.DTOs.Requests;
using ZooTech.Application.Modules.Module_Sanidad.UseCases;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using Xunit;

namespace ZooTech.Application.UnitTests;

public class CreateTriajeUseCaseTests
{
    private readonly Mock<ITriajeRepository> _repositoryMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly CreateTriajeUseCase _useCase;

    public CreateTriajeUseCaseTests()
    {
        _repositoryMock = new Mock<ITriajeRepository>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _useCase = new CreateTriajeUseCase(_repositoryMock.Object, _dateTimeProviderMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_DebeCrearYRetornarTriaje()
    {
        // Arrange
        var request = new TriajeRequest
        {
            VacunoId = 1,
            TipoPesoCode = "CONTROL",
            PesoKg = 150.5m,
            Observaciones = "Todo normal",
            EstadoRegistroCode = "ACTIVO",
            EncargadoUsuarioId = null
        };

        var expectedCodigo = "TRI999";
        var fakeCurrentTime = new DateTime(2026, 5, 20, 15, 0, 0);

        _repositoryMock.Setup(r => r.GenerateCodigoAsync()).ReturnsAsync(expectedCodigo);
        _dateTimeProviderMock.Setup(d => d.ServerNow).Returns(fakeCurrentTime);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCodigo, result.Codigo);
        Assert.Equal(fakeCurrentTime, result.FechaHora);
        Assert.Equal(request.VacunoId, result.VacunoId);
        Assert.Equal(request.TipoPesoCode, result.TipoPesoCode);
        Assert.Equal(request.PesoKg, result.PesoKg);
        Assert.Equal(request.Observaciones, result.Observaciones);

        // Verifica que se haya llamado el AddAsync en el repositorio con los datos correctos
        _repositoryMock.Verify(r => r.AddAsync(It.Is<Triaje>(t => 
            t.Codigo == expectedCodigo &&
            t.VacunoId == request.VacunoId &&
            t.TipoPesoCode == request.TipoPesoCode &&
            t.PesoKg == request.PesoKg
        )), Times.Once);
    }
}
