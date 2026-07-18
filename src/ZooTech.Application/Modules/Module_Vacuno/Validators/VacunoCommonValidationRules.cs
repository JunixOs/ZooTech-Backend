namespace ZooTech.Application.Modules.Module_Vacuno.Validators;

/// <summary>
/// Shared field-validation rules for Vacuno Create/Update commands.
/// Both validators call this to avoid duplicating identical validation logic;
/// only the error-code prefix (CREATE/UPDATE) differs per operation.
/// </summary>
internal static class VacunoCommonValidationRules
{
    public static void ValidateCommonFields(
        List<string> errors,
        string operation,
        string nombre,
        DateOnly fechaNacimiento,
        string tipoAdquisicionCode,
        string razaCode,
        string colorCode,
        string sexoCode,
        long? granjaId,
        string? granja,
        string? codigoDistrito,
        string? observaciones,
        decimal? precioCompra)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            errors.Add($"VACUNO-VACUNO-{operation}-NOMBRE-NULL");
        }

        if (fechaNacimiento == default)
        {
            errors.Add($"VACUNO-VACUNO-{operation}-FECHA_NACIMIENTO-NULL");
        }

        if (string.IsNullOrWhiteSpace(tipoAdquisicionCode))
        {
            errors.Add($"VACUNO-VACUNO-{operation}-TIPO_ADQUISICION_CODE-NULL");
        }

        if (string.IsNullOrWhiteSpace(razaCode))
        {
            errors.Add($"VACUNO-VACUNO-{operation}-RAZA_CODE-NULL");
        }

        if (string.IsNullOrWhiteSpace(colorCode))
        {
            errors.Add($"VACUNO-VACUNO-{operation}-COLOR_CODE-NULL");
        }

        if (string.IsNullOrWhiteSpace(sexoCode))
        {
            errors.Add($"VACUNO-VACUNO-{operation}-SEXO_CODE-NULL");
        }

        var hasExistingGranja = granjaId.HasValue && granjaId.Value > 0;
        var hasNewGranja = !string.IsNullOrWhiteSpace(granja)
            && !string.IsNullOrWhiteSpace(codigoDistrito);

        if (!hasExistingGranja && !hasNewGranja)
        {
            errors.Add($"VACUNO-VACUNO-{operation}-GRANJA_ID-INVALID");
        }

        if (tipoAdquisicionCode == "COMPRA" && !precioCompra.HasValue)
        {
            errors.Add($"VACUNO-VACUNO-{operation}-PRECIO_COMPRA-NULL");
        }
    }
}
