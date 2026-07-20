using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;

namespace ZooTech.Tests.Shared.Factories;

public static class VacunoTestDataFactory
{
    public const long ExistingVacunoId = 1;
    public const long DefaultGranjaId = 1;
    public const string ExistingCodigo = "VAC001";
    public const string DefaultCodigo = "VAC-TST-001";

    public static CreateVacunoCommand CreateCommand() => new(
        Codigo: DefaultCodigo,
        Nombre: "Vacuno Test",
        FechaNacimiento: new DateOnly(2024, 1, 10),
        TipoAdquisicionCode: "NACIMIENTO",
        RazaCode: "HOLSTEIN",
        ColorCode: "NEGRO_BLANCO",
        SexoCode: "HEMBRA",
        CodigoPadre: null,
        CodigoMadre: null,
        GranjaId: DefaultGranjaId,
        Granja: null,
        CodigoDistrito: null,
        Observaciones: "Registro de prueba",
        PrecioCompra: null,
        AptoPara: null);

    public static UpdateVacunoCommand UpdateCommand(long id = ExistingVacunoId) => new(
        Id: id,
        Nombre: "Vacuno Editado",
        FechaNacimiento: new DateOnly(2023, 5, 1),
        TipoAdquisicionCode: "NACIMIENTO",
        RazaCode: "HOLSTEIN",
        ColorCode: "NEGRO_BLANCO",
        SexoCode: "HEMBRA",
        CodigoPadre: null,
        CodigoMadre: null,
        GranjaId: DefaultGranjaId,
        Granja: null,
        CodigoDistrito: null,
        Observaciones: "Actualizacion de prueba",
        PrecioCompra: null,
        AptoPara: null);

    public static Vacuno ExistingVacuno() => Vacuno.Rehydrate(
        id: ExistingVacunoId,
        codigo: ExistingCodigo,
        nombre: "Vacuno Original",
        fechaNacimiento: new DateOnly(2023, 5, 1),
        tipoAdquisicionCode: "NACIMIENTO",
        razaCode: "HOLSTEIN",
        colorCode: "NEGRO_BLANCO",
        sexoCode: "HEMBRA",
        padreId: null,
        madreId: null,
        granjaId: DefaultGranjaId,
        observaciones: null,
        fechaRegistro: new DateOnly(2023, 5, 2),
        createdAt: new DateTime(2023, 5, 2, 12, 0, 0, DateTimeKind.Utc),
        updatedAt: new DateTime(2023, 5, 2, 12, 0, 0, DateTimeKind.Utc),
        deletedAt: null,
        motivoEliminacion: null,
        createdBy: null,
        updatedBy: null,
        deletedBy: null);

    public static Vacuno PersistedFrom(Vacuno vacuno, long id) => Vacuno.Rehydrate(
        id,
        vacuno.Codigo,
        vacuno.Nombre,
        vacuno.FechaNacimiento,
        vacuno.TipoAdquisicionCode,
        vacuno.RazaCode,
        vacuno.ColorCode,
        vacuno.SexoCode,
        vacuno.PadreId,
        vacuno.MadreId,
        vacuno.GranjaId,
        vacuno.Observaciones,
        vacuno.FechaRegistro,
        vacuno.CreatedAt,
        vacuno.UpdatedAt,
        vacuno.DeletedAt,
        vacuno.MotivoEliminacion,
        vacuno.CreatedBy,
        vacuno.UpdatedBy,
        vacuno.DeletedBy);
}
