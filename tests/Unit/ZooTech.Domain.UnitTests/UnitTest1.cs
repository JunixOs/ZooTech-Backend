using ZooTech.Domain.Module_ProduccionLeche.Entities;

namespace ZooTech.Domain.UnitTests;

public class OrdenioTests
{
    [Fact]
    public void CreateNew_WhenLitrosIsZero_ThrowsArgumentException()
    {
        var now = DateTime.UtcNow;

        var action = () => Ordenio.CreateNew(
            codigo: "ORD-001",
            fechaHora: now,
            vacunoId: 1,
            encargadoUsuarioId: 2,
            litros: 0,
            estadoOrdenioCode: "ACTIVO",
            observaciones: null,
            actorUsuarioId: 2,
            utcNow: now);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Contains("litros", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
