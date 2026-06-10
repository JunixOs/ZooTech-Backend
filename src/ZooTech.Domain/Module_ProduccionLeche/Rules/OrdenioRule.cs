using System;

namespace ZooTech.Domain.Module_ProduccionLeche.Rules;

public static class OrdenioRule
{
    public static void ValidarLitros(decimal litros)
    {
        if (litros < 0)
            throw new ArgumentException("La cantidad de litros no puede ser negativa.");
    }

    public static void ValidarFechaHora(DateTime fechaHora)
    {
        if (fechaHora > DateTime.UtcNow)
            throw new ArgumentException("La fecha y hora del ordeño no puede ser en el futuro.");
    }

    public static void ValidarCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("El código del ordeño es obligatorio.");
        if (codigo.Trim().Length > 15)
            throw new ArgumentException("El código del ordeño no puede exceder 15 caracteres.");
    }

    public static void ValidarEstadoOrdenioCode(string estadoOrdenioCode)
    {
        if (string.IsNullOrWhiteSpace(estadoOrdenioCode))
            throw new ArgumentException("El estado del ordeño es obligatorio.");
        if (estadoOrdenioCode.Trim().Length > 30)
            throw new ArgumentException("El estado del ordeño no puede exceder 30 caracteres.");
    }

    public static void ValidarObservaciones(string? observaciones)
    {
        if (!string.IsNullOrWhiteSpace(observaciones) && observaciones.Trim().Length > 150)
            throw new ArgumentException("Las observaciones no pueden exceder 150 caracteres.");
    }

    public static void ValidarMotivoEliminacion(string motivoEliminacion)
    {
        if (string.IsNullOrWhiteSpace(motivoEliminacion))
            throw new ArgumentException("El motivo de eliminación es obligatorio.");
        if (motivoEliminacion.Trim().Length > 200)
            throw new ArgumentException("El motivo de eliminación no puede exceder 200 caracteres.");
    }

    public static void ValidarEncargadoUsuarioId(long encargadoUsuarioId)
    {
        if (encargadoUsuarioId <= 0)
            throw new ArgumentException("El ID del encargado es obligatorio y debe ser un número positivo.");
    }

    public static void ValidarVacunoId(long vacunoId)
    {
        if (vacunoId <= 0)
            throw new ArgumentException("El ID del vacuno es obligatorio y debe ser un número positivo.");
    }
}
