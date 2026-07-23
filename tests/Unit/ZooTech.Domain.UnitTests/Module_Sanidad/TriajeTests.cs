using ZooTech.Domain.Module_Sanidad.Entities;

namespace ZooTech.Domain.UnitTests.Module_Sanidad;

public class TriajeTests
{
    [Fact]
    public void CreateNew_WhenValid_CreatesActiveTriaje()
    {
        var now = DateTime.UtcNow;

        var triaje = Triaje.CreateNew(
            codigo: " TRI001 ",
            fechaHora: now,
            vacunoId: 1,
            tipoPesoCode: " CONTROL ",
            pesoKg: 120.5m,
            observaciones: " Observacion ",
            estadoRegistroCode: " ACTIVO ",
            encargadoUsuarioId: 10,
            utcNow: now);

        Assert.Equal("TRI001", triaje.Codigo);
        Assert.Equal(now, triaje.FechaHora);
        Assert.Equal(1, triaje.VacunoId);
        Assert.Equal("CONTROL", triaje.TipoPesoCode);
        Assert.Equal(120.5m, triaje.PesoKg);
        Assert.Equal("Observacion", triaje.Observaciones);
        Assert.Equal("ACTIVO", triaje.EstadoRegistroCode);
        Assert.Equal(10, triaje.EncargadoUsuarioId);
        Assert.Equal(10, triaje.CreatedBy);
        Assert.Equal(10, triaje.UpdatedBy);
        Assert.False(triaje.IsDeleted);
    }

    [Fact]
    public void CreateNew_WhenFechaHoraIsFuture_ThrowsArgumentException()
    {
        var now = DateTime.UtcNow;

        var action = () => CreateValid(fechaHora: now.AddMinutes(5), utcNow: now);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Contains("futura", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateNew_WhenPesoKgIsZero_ThrowsArgumentException()
    {
        var action = () => CreateValid(pesoKg: 0);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Contains("peso", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateNew_WhenVacunoIdIsInvalid_ThrowsArgumentException()
    {
        var action = () => CreateValid(vacunoId: 0);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Contains("vacuno", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateNew_WhenTipoPesoCodeIsEmpty_ThrowsArgumentException()
    {
        var action = () => CreateValid(tipoPesoCode: string.Empty);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Contains("tipo de peso", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateNew_WhenObservacionesExceeds150_TruncatesValue()
    {
        var observaciones = new string('A', 151);

        var triaje = CreateValid(observaciones: observaciones);

        Assert.NotNull(triaje.Observaciones);
        Assert.Equal(150, triaje.Observaciones.Length);
    }

    [Fact]
    public void Update_WhenValid_UpdatesFieldsAndAudit()
    {
        var createdAt = DateTime.UtcNow.AddMinutes(-10);
        var updatedAt = DateTime.UtcNow;
        var triaje = CreateValid(utcNow: createdAt);

        triaje.Update("FINAL", 155.75m, " Nuevo valor ", 20, updatedAt);

        Assert.Equal("FINAL", triaje.TipoPesoCode);
        Assert.Equal(155.75m, triaje.PesoKg);
        Assert.Equal("Nuevo valor", triaje.Observaciones);
        Assert.Equal(20, triaje.EncargadoUsuarioId);
        Assert.Equal(20, triaje.UpdatedBy);
        Assert.Equal(updatedAt, triaje.UpdatedAt);
    }

    [Fact]
    public void Update_WhenTriajeIsDeleted_ThrowsInvalidOperationException()
    {
        var triaje = CreateValid();
        triaje.SoftDelete("Duplicado", "ELIMINADO", 10, DateTime.UtcNow);

        var action = () => triaje.Update("FINAL", 155.75m, null, 20, DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void SoftDelete_WhenMotivoIsEmpty_ThrowsArgumentException()
    {
        var triaje = CreateValid();

        var action = () => triaje.SoftDelete(string.Empty, "ELIMINADO", 10, DateTime.UtcNow);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Contains("motivo", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SoftDelete_WhenAlreadyDeleted_ThrowsInvalidOperationException()
    {
        var triaje = CreateValid();
        triaje.SoftDelete("Duplicado", "ELIMINADO", 10, DateTime.UtcNow);

        var action = () => triaje.SoftDelete("Otro motivo", "ELIMINADO", 10, DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void SoftDelete_WhenValid_SetsDeletedFields()
    {
        var triaje = CreateValid();
        var deletedAt = DateTime.UtcNow;

        triaje.SoftDelete("Duplicado", "ELIMINADO", 10, deletedAt);

        Assert.True(triaje.IsDeleted);
        Assert.Equal("Duplicado", triaje.MotivoEliminacion);
        Assert.Equal("ELIMINADO", triaje.EstadoRegistroCode);
        Assert.Equal(deletedAt, triaje.DeletedAt);
        Assert.Equal(10, triaje.DeletedBy);
        Assert.Equal(10, triaje.UpdatedBy);
        Assert.Equal(deletedAt, triaje.UpdatedAt);
    }

    private static Triaje CreateValid(
        long vacunoId = 1,
        string tipoPesoCode = "CONTROL",
        decimal pesoKg = 120.5m,
        string? observaciones = null,
        DateTime? fechaHora = null,
        DateTime? utcNow = null)
    {
        var now = utcNow ?? DateTime.UtcNow;
        return Triaje.CreateNew(
            codigo: "TRI001",
            fechaHora: fechaHora ?? now,
            vacunoId: vacunoId,
            tipoPesoCode: tipoPesoCode,
            pesoKg: pesoKg,
            observaciones: observaciones,
            estadoRegistroCode: "ACTIVO",
            encargadoUsuarioId: 10,
            utcNow: now);
    }
}
