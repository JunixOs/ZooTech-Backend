using ZooTech.Domain.Module_Celo.Entities;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;

internal static class CeloReporteItemMapper
{
    public static CeloReporteItemDto Map(
        CeloReporteItem celo,
        IReadOnlyDictionary<long, int> vecesEnCeloCounts,
        IReadOnlyDictionary<long, int> criasCounts)
    {
        return new CeloReporteItemDto
        {
            CodigoRegistro = celo.Codigo,
            Fecha = DateOnly.FromDateTime(celo.FechaHora),
            Hora = TimeOnly.FromDateTime(celo.FechaHora),
            CodigoVacuno = celo.VacunoCodigo,
            NombreVacuno = celo.NombreVacuno,
            VecesEnCelo = vecesEnCeloCounts.GetValueOrDefault(celo.VacunoId, 1),
            Caracteristicas = celo.CaracteristicaCodes.Count,
            ListaCaracteristicas = celo.CaracteristicaCodes,
            Observaciones = celo.Observaciones,
            Crias = criasCounts.GetValueOrDefault(celo.VacunoId, 0)
        };
    }
}
