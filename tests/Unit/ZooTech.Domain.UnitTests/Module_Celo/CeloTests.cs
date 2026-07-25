using ZooTech.Domain.Module_Celo.Entities;

namespace ZooTech.Domain.UnitTests.Module_Celo;

public class CeloTests
{
    [Fact]
    public void CreateNew_WhenValid_CreatesActiveCelo()
    {
        var now = DateTime.UtcNow;

        var celo = CreateValidCelo(utcNow: now, fechaHora: now.AddHours(-1));

        Assert.False(celo.IsDeleted);
        Assert.Equal(["CALOR", "MOUNT"], celo.CaracteristicaCodes);
    }

    [Fact]
    public void CreateNew_WhenCodigoIsEmpty_ThrowsArgumentException()
    {
        var action = () => CreateValidCelo(codigo: string.Empty);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("El código del registro de celo es obligatorio.", exception.Message);
    }

    [Fact]
    public void CreateNew_WhenCodigoExceedsMaxLength_ThrowsArgumentException()
    {
        var action = () => CreateValidCelo(codigo: new string('a', 16));

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("El código del registro de celo no puede superar los 15 caracteres.", exception.Message);
    }

    [Fact]
    public void CreateNew_WhenFechaHoraIsFuture_ThrowsArgumentException()
    {
        var now = DateTime.UtcNow;

        var action = () => CreateValidCelo(fechaHora: now.AddMinutes(5), utcNow: now);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("La fecha y hora del registro de celo no puede ser futura.", exception.Message);
    }

    [Fact]
    public void CreateNew_WhenVacunoIdIsNotPositive_ThrowsArgumentException()
    {
        var action = () => CreateValidCelo(vacunoId: 0);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("El ID del vacuno debe ser mayor a 0.", exception.Message);
    }

    [Fact]
    public void CreateNew_WhenEncargadoUsuarioIdIsNotPositive_ThrowsArgumentException()
    {
        var action = () => CreateValidCelo(encargadoUsuarioId: 0);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("El ID del encargado debe ser mayor a 0.", exception.Message);
    }

    [Fact]
    public void CreateNew_WhenEstadoRegistroCodeIsEmpty_ThrowsArgumentException()
    {
        var action = () => CreateValidCelo(estadoRegistroCode: string.Empty);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("El código de estado de registro es obligatorio.", exception.Message);
    }

    [Fact]
    public void CreateNew_WhenEstadoRegistroCodeExceedsMaxLength_ThrowsArgumentException()
    {
        var action = () => CreateValidCelo(estadoRegistroCode: new string('a', 31));

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("El código de estado de registro no puede superar los 30 caracteres.", exception.Message);
    }

    [Fact]
    public void CreateNew_WhenObservacionesExceedsMaxLength_ThrowsArgumentException()
    {
        var action = () => CreateValidCelo(observaciones: new string('a', 151));

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("Las observaciones no pueden superar los 150 caracteres.", exception.Message);
    }

    [Fact]
    public void Update_WhenValid_UpdatesObservacionesCaracteristicasAndAudit()
    {
        var createdAt = DateTime.UtcNow.AddHours(-2);
        var updatedAt = DateTime.UtcNow;
        var celo = CreateValidCelo(utcNow: createdAt, fechaHora: createdAt);

        celo.Update("Nueva observación", ["MUGIDO"], actorUsuarioId: 99, utcNow: updatedAt);

        Assert.Equal("Nueva observación", celo.Observaciones);
        Assert.Equal(["MUGIDO"], celo.CaracteristicaCodes);
        Assert.Equal(99, celo.UpdatedBy);
        Assert.Equal(updatedAt, celo.UpdatedAt);
    }

    [Fact]
    public void Update_WhenCeloIsDeleted_ThrowsInvalidOperationException()
    {
        var celo = CreateValidCelo();
        celo.SoftDelete("Duplicado", 10, DateTime.UtcNow);

        var action = () => celo.Update("Otra observación", null, 10, DateTime.UtcNow);

        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("No se puede actualizar un registro de celo eliminado.", exception.Message);
    }

    [Fact]
    public void Update_WhenObservacionesExceedMaxLength_ThrowsArgumentException()
    {
        var celo = CreateValidCelo();

        var action = () => celo.Update(new string('a', 151), null, 10, DateTime.UtcNow);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("Las observaciones no pueden superar los 150 caracteres.", exception.Message);
    }

    [Fact]
    public void SoftDelete_WhenValid_SetsDeletedFieldsAndTrimsMotivo()
    {
        var celo = CreateValidCelo();
        var deletedAt = DateTime.UtcNow;

        celo.SoftDelete("  Duplicado  ", 10, deletedAt);

        Assert.True(celo.IsDeleted);
        Assert.Equal("Duplicado", celo.MotivoEliminacion);
        Assert.Equal(deletedAt, celo.DeletedAt);
        Assert.Equal(10, celo.DeletedBy);
        Assert.Equal(10, celo.UpdatedBy);
        Assert.Equal(deletedAt, celo.UpdatedAt);
    }

    [Fact]
    public void SoftDelete_WhenAlreadyDeleted_ThrowsInvalidOperationException()
    {
        var celo = CreateValidCelo();
        celo.SoftDelete("Duplicado", 10, DateTime.UtcNow);

        var action = () => celo.SoftDelete("Otro motivo", 10, DateTime.UtcNow);

        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("El registro de celo ya se encuentra eliminado.", exception.Message);
    }

    [Fact]
    public void SoftDelete_WhenMotivoIsEmpty_ThrowsArgumentException()
    {
        var celo = CreateValidCelo();

        var action = () => celo.SoftDelete(string.Empty, 10, DateTime.UtcNow);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("El motivo de eliminación es obligatorio.", exception.Message);
    }

    [Fact]
    public void SoftDelete_WhenMotivoExceedsMaxLength_ThrowsArgumentException()
    {
        var celo = CreateValidCelo();

        var action = () => celo.SoftDelete(new string('a', 201), 10, DateTime.UtcNow);

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("El motivo de eliminación no puede superar los 200 caracteres.", exception.Message);
    }

    [Fact]
    public void CreateNew_WhenObservacionesIsNullOrWhitespace_SanitizesToNull()
    {
        var celoNull = CreateValidCelo(observaciones: null);
        var celoWhitespace = CreateValidCelo(observaciones: "   ");

        Assert.Null(celoNull.Observaciones);
        Assert.Null(celoWhitespace.Observaciones);
    }

    [Fact]
    public void CreateNew_WhenObservacionesIsValid_TrimsValue()
    {
        var celo = CreateValidCelo(observaciones: "  Observación válida  ");

        Assert.Equal("Observación válida", celo.Observaciones);
    }

    private static Celo CreateValidCelo(
        string codigo = "CEL-001",
        DateTime? fechaHora = null,
        long vacunoId = 1,
        long encargadoUsuarioId = 2,
        string? observaciones = "Observación",
        string estadoRegistroCode = "ACTIVO",
        List<string>? caracteristicaCodes = null,
        long? actorUsuarioId = 2,
        DateTime? utcNow = null)
    {
        var now = utcNow ?? DateTime.UtcNow;
        return Celo.CreateNew(
            codigo: codigo,
            fechaHora: fechaHora ?? now,
            vacunoId: vacunoId,
            encargadoUsuarioId: encargadoUsuarioId,
            observaciones: observaciones,
            estadoRegistroCode: estadoRegistroCode,
            caracteristicaCodes: caracteristicaCodes ?? ["CALOR", "MOUNT"],
            actorUsuarioId: actorUsuarioId,
            utcNow: now);
    }
}
