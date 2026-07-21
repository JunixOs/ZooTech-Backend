namespace ZooTech.Domain.Ganaderia.Module_Vacuno.Rules;

public static class VacunoRule
{
    public static void ValidarIdPersistido(long id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID persistido del vacuno debe ser mayor a 0.");
    }

    public static void ValidarCampoPersistido(string? valor, string nombreCampo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException($"El campo persistido {nombreCampo} del vacuno es obligatorio.");
    }

    public static void ValidarFechasPersistidas(
        DateOnly fechaNacimiento,
        DateOnly fechaRegistro,
        DateTime createdAt,
        DateTime updatedAt,
        DateTime? deletedAt)
    {
        if (fechaNacimiento == default)
            throw new ArgumentException("La fecha de nacimiento persistida del vacuno es obligatoria.");

        if (fechaRegistro == default)
            throw new ArgumentException("La fecha de registro persistida del vacuno es obligatoria.");

        if (createdAt == default)
            throw new ArgumentException("La fecha de creacion persistida del vacuno es obligatoria.");

        if (updatedAt == default)
            throw new ArgumentException("La fecha de actualizacion persistida del vacuno es obligatoria.");

        if (updatedAt < createdAt)
            throw new ArgumentException("La fecha de actualizacion persistida no puede ser anterior a la creacion.");

        if (deletedAt.HasValue && deletedAt.Value < createdAt)
            throw new ArgumentException("La fecha de eliminacion persistida no puede ser anterior a la creacion.");
    }

    public static void ValidarCodigo(string? codigo, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("El codigo del vacuno es obligatorio.");

        if (codigo.Trim().Length > maxLength)
            throw new ArgumentException($"El codigo del vacuno no puede superar los {maxLength} caracteres.");
    }

    public static void ValidarNombre(string? nombre, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del vacuno es obligatorio.");

        if (nombre.Trim().Length > maxLength)
            throw new ArgumentException($"El nombre del vacuno no puede superar los {maxLength} caracteres.");
    }

    public static void ValidarGranjaId(long granjaId)
    {
        if (granjaId <= 0)
            throw new ArgumentException("El ID de la granja debe ser mayor a 0.");
    }

    public static void ValidarRazaCode(string? razaCode, int maxLength)
    {
        ValidarCatalogoCode(razaCode, "raza", maxLength);
    }

    public static void ValidarSexoCode(string? sexoCode, int maxLength)
    {
        ValidarCatalogoCode(sexoCode, "sexo", maxLength);
    }

    public static void ValidarTipoAdquisicionCode(string? tipoAdquisicionCode, int maxLength)
    {
        ValidarCatalogoCode(tipoAdquisicionCode, "tipo de adquisicion", maxLength);
    }

    public static void ValidarColorCode(string? colorCode, int maxLength)
    {
        ValidarCatalogoCode(colorCode, "color", maxLength);
    }

    public static void ValidarObservaciones(string? observaciones, int maxLength, int maxWords)
    {
        if (string.IsNullOrWhiteSpace(observaciones))
            return;

        var value = observaciones.Trim();
        if (value.Length > maxLength)
            throw new ArgumentException($"Las observaciones no pueden superar los {maxLength} caracteres.");

        if (value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length > maxWords)
            throw new ArgumentException($"Las observaciones no pueden superar las {maxWords} palabras.");
    }

    public static void ValidarMotivoEliminacion(string? motivoEliminacion)
    {
        if (string.IsNullOrWhiteSpace(motivoEliminacion))
            throw new ArgumentException("El motivo de eliminacion es obligatorio.");

        if (motivoEliminacion.Trim().Length > 200)
            throw new ArgumentException("El motivo de eliminacion no puede superar los 200 caracteres.");
    }

    private static void ValidarCatalogoCode(string? value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"El codigo de {fieldName} es obligatorio.");

        if (value.Trim().Length > maxLength)
            throw new ArgumentException($"El codigo de {fieldName} no puede superar los {maxLength} caracteres.");
    }
}
