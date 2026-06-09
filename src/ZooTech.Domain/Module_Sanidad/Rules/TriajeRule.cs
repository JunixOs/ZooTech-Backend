using System;

namespace ZooTech.Domain.Module_Sanidad.Rules
{
    public class TriajeRule
    {
        public static void ValidarPesoKg(decimal pesoKg)
        {
            if (pesoKg <= 0)
                throw new ArgumentException("El peso debe ser mayor a cero.", nameof(pesoKg));
        }

        public static void ValidarObservaciones(string? observaciones)
        {
            if (!string.IsNullOrWhiteSpace(observaciones) && observaciones.Trim().Length > 150)
                throw new ArgumentException("Las observaciones no pueden exceder 150 caracteres.");
        }

        public static void ValidarVacunoId(long vacunoId)
        {
            if (vacunoId <= 0)
                throw new ArgumentException("El ID del vacuno es obligatorio y debe ser un número positivo.");
        }

        public static void ValidarTipoPesoCode(string tipoPesoCode)
        {
            if (string.IsNullOrWhiteSpace(tipoPesoCode))
                throw new ArgumentException("El tipo de peso es obligatorio.");
        }
    }
}
