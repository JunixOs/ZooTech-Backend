using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.Validators;

internal static class VacunoValidationErrorDetails
{
    private static readonly (string Token, string Field)[] FieldMappings =
    [
        ("FECHA_NACIMIENTO", "fechaNacimiento"),
        ("TIPO_ADQUISICION_CODE", "tipoAdquisicionCode"),
        ("PRECIO_COMPRA", "precioCompra"),
        ("CODIGO_DISTRITO", "codigoDistrito"),
        ("CODIGO_PADRE", "codigoPadre"),
        ("CODIGO_MADRE", "codigoMadre"),
        ("GRANJA_ID", "granjaId"),
        ("RAZA_CODE", "razaCode"),
        ("COLOR_CODE", "colorCode"),
        ("SEXO_CODE", "sexoCode"),
        ("OBSERVACIONES", "observaciones"),
        ("CODIGO", "codigo"),
        ("NOMBRE", "nombre"),
        ("ID", "id")
    ];

    public static IReadOnlyList<FieldValidationError> FromCodes(
        IReadOnlyCollection<string> errorCodes)
        => errorCodes
            .Select(code => new FieldValidationError(
                ResolveField(code),
                code,
                "Campo obligatorio o invalido."))
            .ToList();

    public static ValidationException CreateException(
        string field,
        string code,
        string message)
        => new(
            [code],
            ScopeName.Application,
            ModuleName.Vacuno,
            [new FieldValidationError(field, code, message)],
            message);

    private static string ResolveField(string code)
        => FieldMappings
            .FirstOrDefault(mapping => code.Contains($"-{mapping.Token}-", StringComparison.Ordinal))
            .Field ?? "formulario";
}
