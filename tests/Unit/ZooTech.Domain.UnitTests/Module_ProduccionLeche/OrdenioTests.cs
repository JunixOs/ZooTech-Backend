using ZooTech.Domain.Module_ProduccionLeche.Entities;

namespace ZooTech.Domain.UnitTests.Module_ProduccionLeche;

public class OrdenioTests
{
    [Fact]
    public void CreateNew_WhenFechaHoraIsFuture_ThrowsArgumentException()
    {
        var now = DateTime.UtcNow;

        var action = () => Ordenio.CreateNew(
            codigo: "ORD-001",
            fechaHora: DateTime.UtcNow.AddMinutes(1),
            vacunoId: 1,
            encargadoUsuarioId: 2,
            litros: 10,
            estadoOrdenioCode: "ACTIVO",
            observaciones: null,
            actorUsuarioId: 2,
            utcNow: now);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Contains("fecha y hora", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

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

    [Fact]
    public void Update_WhenDataIsValid_UpdatesEditableFields()
    {
        var createdAt = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var newFechaHora = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var utcNow = new DateTime(2024, 1, 3, 9, 0, 0, DateTimeKind.Utc);

        var ordenio = Ordenio.Rehydrate(
            id: 1,
            codigo: "ORD-001",
            fechaHora: createdAt,
            vacunoId: 10,
            nombreVacuno: "Vacuno 1",
            encargadoUsuarioId: 20,
            nombreCompleto: "Juan Perez",
            litros: 12,
            estadoOrdenioCode: "ACTIVO",
            observaciones: "estado inicial",
            createdAt: createdAt,
            updatedAt: createdAt,
            deletedAt: null,
            motivoEliminacion: null,
            createdBy: 1,
            updatedBy: 1,
            deletedBy: null);

        ordenio.Update(
            fechaHora: newFechaHora,
            encargadoUsuarioId: 30,
            litros: 15,
            estadoOrdenioCode: "INACTIVO",
            observaciones: "estado inicial desactivado",
            actorUsuarioId: 99,
            utcNow: utcNow);

        Assert.Equal(newFechaHora, ordenio.FechaHora);
        Assert.Equal(30, ordenio.EncargadoUsuarioId);
        Assert.Equal(15, ordenio.Litros);
        Assert.Equal("INACTIVO", ordenio.EstadoOrdenioCode);
        Assert.Equal("estado inicial desactivado", ordenio.Observaciones);
        Assert.Equal(utcNow, ordenio.UpdatedAt);
        Assert.Equal(99, ordenio.UpdatedBy);
    }
}
