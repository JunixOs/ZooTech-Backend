using System;
using System.Collections.Generic;
using System.Text;


using ZooTech.Domain.Entities.Configuration;

namespace ZooTech.Domain.Module_ProduccionLeche.Rules
{
    public class OrdenioRule
    {
        public OrdenioRule() { }


        public static void ValidarLitros(decimal litros)
        {
            if (litros < 0)
                throw new ArgumentException(ConfigSettings.Produccionleche.MilkingLitersNegativeError);
        }

        public static void ValidarFechaHora(DateTime fechaHora)
        {
            if (fechaHora > DateTime.UtcNow)
                throw new ArgumentException(ConfigSettings.Produccionleche.MilkingDatetimeFutureError);
        }

        public static void ValidarCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException(ConfigSettings.Produccionleche.MilkingCodeRequiredError);
            if (codigo.Trim().Length > 15)
                throw new ArgumentException(ConfigSettings.Produccionleche.MilkingCodeMaxLengthError);
        }

        public static void ValidarEstadoOrdenioCode(string estadoOrdenioCode)
        {
            if (string.IsNullOrWhiteSpace(estadoOrdenioCode))
                throw new ArgumentException(ConfigSettings.Produccionleche.MilkingStatusMaxLengthError);
            if (estadoOrdenioCode.Trim().Length > 30)
                throw new ArgumentException(ConfigSettings.Produccionleche.MilkingStatusRequiredError);
        }

        public static void ValidarObservaciones(string? observaciones)
        {
            if (!string.IsNullOrWhiteSpace(observaciones) && observaciones.Trim().Length > 150)
                throw new ArgumentException(ConfigSettings.Produccionleche.MilkingObservationsMaxLengthError);
        }

        public static void ValidarMotivoEliminacion(string motivoEliminacion)
        {
            if (string.IsNullOrWhiteSpace(motivoEliminacion))
                throw new ArgumentException(ConfigSettings.Produccionleche.MilkingDeleteReasonRequiredError);
            if (motivoEliminacion.Trim().Length > 200)
                throw new ArgumentException(ConfigSettings.Produccionleche.MilkingDeleteReasonMaxLengthError);
        }
        public static void ValidarEncargadoUsuarioId(long encargadoUsuarioId)
        {
            if (encargadoUsuarioId <= 0)
                throw new ArgumentException(ConfigSettings.Produccionleche.MilkingManagerRequiredError);
        }
        public static void ValidarVacunoId(long vacunoId)
        {
            if (vacunoId <= 0)
                throw new ArgumentException(ConfigSettings.Produccionleche.MilkingCattleRequiredError);
        }
    }

}