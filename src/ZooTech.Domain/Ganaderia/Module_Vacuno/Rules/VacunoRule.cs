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
            throw new ArgumentException("La fecha de creación persistida del vacuno es obligatoria.");

        if (updatedAt == default)
            throw new ArgumentException("La fecha de actualización persistida del vacuno es obligatoria.");

        if (updatedAt < createdAt)
            throw new ArgumentException("La fecha de actualización persistida no puede ser anterior a la creación.");

        if (deletedAt.HasValue && deletedAt.Value < createdAt)
            throw new ArgumentException("La fecha de eliminación persistida no puede ser anterior a la creación.");
    }

    public static void ValidarCodigo(string? codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("El código del vacuno es obligatorio.");

        if (codigo.Trim().Length > 15)
            throw new ArgumentException("El código del vacuno no puede superar los 15 caracteres.");
    }

    public static void ValidarNombre(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del vacuno es obligatorio.");

        if (nombre.Trim().Length > 100)
            throw new ArgumentException("El nombre del vacuno no puede superar los 100 caracteres.");
    }

    public static void ValidarGranjaId(long granjaId)
    {
        if (granjaId <= 0)
            throw new ArgumentException("El ID de la granja debe ser mayor a 0.");
    }

    public static void ValidarRazaCode(string? razaCode)
    {
        if (string.IsNullOrWhiteSpace(razaCode))
            throw new ArgumentException("El código de raza es obligatorio.");

        if (razaCode.Trim().Length > 30)
            throw new ArgumentException("El código de raza no puede superar los 30 caracteres.");
    }

    public static void ValidarSexoCode(string? sexoCode)
    {
        if (string.IsNullOrWhiteSpace(sexoCode))
            throw new ArgumentException("El código de sexo es obligatorio.");

        if (sexoCode.Trim().Length > 10)
            throw new ArgumentException("El código de sexo no puede superar los 10 caracteres.");
    }

    public static void ValidarTipoAdquisicionCode(string? tipoAdquisicionCode)
    {
        if (string.IsNullOrWhiteSpace(tipoAdquisicionCode))
            throw new ArgumentException("El código de tipo de adquisición es obligatorio.");

        if (tipoAdquisicionCode.Trim().Length > 30)
            throw new ArgumentException("El código de tipo de adquisición no puede superar los 30 caracteres.");
    }

    public static void ValidarColorCode(string? colorCode)
    {
        if (string.IsNullOrWhiteSpace(colorCode))
            throw new ArgumentException("El código de color es obligatorio.");

        if (colorCode.Trim().Length > 30)
            throw new ArgumentException("El código de color no puede superar los 30 caracteres.");
    }

    public static void ValidarObservaciones(string? observaciones)
    {
        if (!string.IsNullOrWhiteSpace(observaciones) && observaciones.Trim().Length > 150)
            throw new ArgumentException("Las observaciones no pueden superar los 150 caracteres.");
    }

    public static void ValidarMotivoEliminacion(string? motivoEliminacion)
    {
        if (string.IsNullOrWhiteSpace(motivoEliminacion))
            throw new ArgumentException("El motivo de eliminación es obligatorio.");

        if (motivoEliminacion.Trim().Length > 200)
            throw new ArgumentException("El motivo de eliminación no puede superar los 200 caracteres.");
    }
}
