namespace ZooTech.Domain.Module_Celo.Rules;

public static class CeloRule
{
    public static void ValidarCodigo(string? codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            throw new ArgumentException("El código del registro de celo es obligatorio.");
        }

        if (codigo.Trim().Length > 15)
        {
            throw new ArgumentException("El código del registro de celo no puede superar los 15 caracteres.");
        }
    }

    public static void ValidarFechaHora(DateTime fechaHora)
    {
        if (fechaHora > DateTime.UtcNow)
        {
            throw new ArgumentException("La fecha y hora del registro de celo no puede ser futura.");
        }
    }

    public static void ValidarVacunoId(long vacunoId)
    {
        if (vacunoId <= 0)
        {
            throw new ArgumentException("El ID del vacuno debe ser mayor a 0.");
        }
    }

    public static void ValidarEncargadoUsuarioId(long encargadoUsuarioId)
    {
        if (encargadoUsuarioId <= 0)
        {
            throw new ArgumentException("El ID del encargado debe ser mayor a 0.");
        }
    }

    public static void ValidarEstadoRegistroCode(string? estadoRegistroCode)
    {
        if (string.IsNullOrWhiteSpace(estadoRegistroCode))
        {
            throw new ArgumentException("El código de estado de registro es obligatorio.");
        }

        if (estadoRegistroCode.Trim().Length > 30)
        {
            throw new ArgumentException("El código de estado de registro no puede superar los 30 caracteres.");
        }
    }

    public static void ValidarObservaciones(string? observaciones)
    {
        if (!string.IsNullOrWhiteSpace(observaciones) && observaciones.Trim().Length > 150)
        {
            throw new ArgumentException("Las observaciones no pueden superar los 150 caracteres.");
        }
    }

    public static void ValidarMotivoEliminacion(string? motivoEliminacion)
    {
        if (string.IsNullOrWhiteSpace(motivoEliminacion))
        {
            throw new ArgumentException("El motivo de eliminación es obligatorio.");
        }

        if (motivoEliminacion.Trim().Length > 200)
        {
            throw new ArgumentException("El motivo de eliminación no puede superar los 200 caracteres.");
        }
    }
}
