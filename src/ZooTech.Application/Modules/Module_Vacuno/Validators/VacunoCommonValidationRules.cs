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
        long granjaId,
        string? observaciones,
        decimal? precioCompra)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            errors.Add($"VACUNO-VACUNO-{operation}-NOMBRE-NULL");
        }
        else if (nombre.Length > 15)
        {
            errors.Add($"VACUNO-VACUNO-{operation}-NOMBRE-INVALID");
        }

        if (fechaNacimiento == default)
        {
            errors.Add($"VACUNO-VACUNO-{operation}-FECHA_NACIMIENTO-NULL");
        }

        if (string.IsNullOrWhiteSpace(tipoAdquisicionCode))
        {
            errors.Add($"VACUNO-VACUNO-{operation}-TIPO_ADQUISICION_CODE-NULL");
        }
        else if (tipoAdquisicionCode.Length > 30)
        {
            errors.Add($"VACUNO-VACUNO-{operation}-TIPO_ADQUISICION_CODE-INVALID");
        }

        if (string.IsNullOrWhiteSpace(razaCode))
        {
            errors.Add($"VACUNO-VACUNO-{operation}-RAZA_CODE-NULL");
        }
        else if (razaCode.Length > 30)
        {
            errors.Add($"VACUNO-VACUNO-{operation}-RAZA_CODE-INVALID");
        }

        if (string.IsNullOrWhiteSpace(colorCode))
        {
            errors.Add($"VACUNO-VACUNO-{operation}-COLOR_CODE-NULL");
        }
        else if (colorCode.Length > 30)
        {
            errors.Add($"VACUNO-VACUNO-{operation}-COLOR_CODE-INVALID");
        }

        if (string.IsNullOrWhiteSpace(sexoCode))
        {
            errors.Add($"VACUNO-VACUNO-{operation}-SEXO_CODE-NULL");
        }
        else if (sexoCode.Length > 10)
        {
            errors.Add($"VACUNO-VACUNO-{operation}-SEXO_CODE-INVALID");
        }

        if (granjaId <= 0)
        {
            errors.Add($"VACUNO-VACUNO-{operation}-GRANJA_ID-INVALID");
        }

        if (!string.IsNullOrWhiteSpace(observaciones))
        {
            if (observaciones.Length > 150)
            {
                errors.Add($"VACUNO-VACUNO-{operation}-OBSERVACIONES-INVALID");
            }

            if (observaciones
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Length > 30)
            {
                errors.Add($"VACUNO-VACUNO-{operation}-OBSERVACIONES-WORD_LIMIT");
            }
        }

        if (tipoAdquisicionCode == "COMPRA" && !precioCompra.HasValue)
        {
            errors.Add($"VACUNO-VACUNO-{operation}-PRECIO_COMPRA-NULL");
        }
    }
}
