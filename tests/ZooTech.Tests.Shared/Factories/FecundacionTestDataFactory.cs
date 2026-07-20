using ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;

namespace ZooTech.Tests.Shared.Factories;

public static class FecundacionTestDataFactory
{
    public const long ExistingFecundacionId = 10;

    public static DeleteFecundacionCommand DeleteCommand(
        long id = ExistingFecundacionId,
        string razon = "Registro duplicado") => new(id, razon);

    public static FecundacionEditData EditData(long id = ExistingFecundacionId) => new(
        id,
        "FEC001",
        "INSEMINACION_ARTIFICIAL",
        1,
        "VAC001",
        "Estrella",
        "INTERNO",
        2,
        "VAC002",
        "Toro Rey",
        null,
        null,
        new DateOnly(2026, 1, 11),
        "Dr. Prueba",
        "EXITOSO",
        "CONFIRMADA",
        "Procedimiento sin complicaciones",
        "SEM-001",
        null,
        new DateTime(2026, 1, 11, 12, 0, 0, DateTimeKind.Utc),
        new DateTime(2026, 1, 11, 12, 0, 0, DateTimeKind.Utc));
}
