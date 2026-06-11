namespace ZooTech.Domain.Module_Sanidad.Rules;

public static class TriajeRule
{
    public static void ValidarVacunoId(long vacunoId)
    {
        if (vacunoId <= 0)
            throw new ArgumentException("El ID del vacuno debe ser mayor que cero.", nameof(vacunoId));
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

    public static void ValidarEstadoRegistroCode(string estadoRegistroCode)
    {
        if (string.IsNullOrWhiteSpace(estadoRegistroCode))
            throw new ArgumentException("El estado de registro es obligatorio.", nameof(estadoRegistroCode));
    }

    public static void ValidarFechaHora(DateTime fechaHora)
    {
        if (fechaHora == default)
            throw new ArgumentException("La fecha y hora del triaje es obligatoria.", nameof(fechaHora));
    }
}
