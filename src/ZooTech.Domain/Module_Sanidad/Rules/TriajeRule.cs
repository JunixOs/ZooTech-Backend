namespace ZooTech.Domain.Module_Sanidad.Rules;

public static class TriajeRule
{
    public static void ValidarVacunoId(long vacunoId)
    {
        if (vacunoId <= 0)
            throw new ArgumentException("El vacuno debe ser obligatorio.", nameof(vacunoId));
    }

    public static void ValidarTipoPesoCode(string tipoPesoCode)
    {
        if (string.IsNullOrWhiteSpace(tipoPesoCode))
            throw new ArgumentException("El tipo de peso es obligatorio.", nameof(tipoPesoCode));
    }

    public static void ValidarPesoKg(decimal pesoKg)
    {
        if (pesoKg <= 0)
            throw new ArgumentException("El peso debe ser mayor que cero.", nameof(pesoKg));
    }


    public static void ValidarFechaHora(DateTime fechaHora)
    {
        if (fechaHora == default)
            throw new ArgumentException("La fecha y hora del triaje es obligatoria.", nameof(fechaHora));
    }

    public static void ValidarFechaHoraFutura(DateTime fechaHora)
    {
        if (fechaHora > DateTime.UtcNow)
            throw new ArgumentException("La fecha y hora del triaje no puede ser futura.", nameof(fechaHora));
    }

    public static void ValidarMotivoEliminacion(string motivoEliminacion)
    {
        if (string.IsNullOrWhiteSpace(motivoEliminacion))
            throw new ArgumentException("El motivo de eliminación es obligatorio.", nameof(motivoEliminacion));
    }
}
