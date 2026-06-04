namespace ZooTech.Domain.Module_Sanidad.Rules;

public class TriajeRule
{
    // Validar el Peso (No puede ser 0 o menor)
    public static void ValidarPesoKg(decimal pesoKg)
    {
        if (pesoKg <= 0)
            throw new ArgumentException("El peso registrado debe ser mayor a 0 kg.");
        
        // Regla de cordura: si es Triaje vacuno (una vaca no pesa 10,000 kg).
        if (pesoKg > 2000)
            throw new ArgumentException("El peso registrado excede un valor razonable para un vacuno (máx 2000 kg).");
    }

    // Validar las observaciones
    public static void ValidarObservaciones(string? observaciones)
    {
        if (!string.IsNullOrWhiteSpace(observaciones) && observaciones.Trim().Length > 250)
            throw new ArgumentException("Las observaciones no pueden exceder los 250 caracteres.");
    }

    // Validar el Código de Estado (Sano, Enfermo, etc)
    public static void ValidarEstadoRegistroCode(string estadoRegistroCode)
    {
        if (string.IsNullOrWhiteSpace(estadoRegistroCode))
            throw new ArgumentException("El estado del registro es obligatorio.");
            
        if (estadoRegistroCode.Trim().Length > 20)
            throw new ArgumentException("El estado del registro no puede exceder los 20 caracteres.");
    }
}
