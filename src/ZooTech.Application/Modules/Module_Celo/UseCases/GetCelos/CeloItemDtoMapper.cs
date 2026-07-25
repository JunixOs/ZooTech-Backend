using ZooTech.Domain.Module_Celo.Entities;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;

internal static class CeloItemDtoMapper
{
    public static CeloItemDto Map(
        CeloListItem celo,
        IReadOnlyDictionary<long, int> vecesEnCeloCounts)
    {
        return new CeloItemDto
        {
            Id = celo.Id,

            CodigoRegistro = celo.Codigo,
            Fecha = DateOnly.FromDateTime(celo.FechaHora),
            Hora = TimeOnly.FromDateTime(celo.FechaHora),
            CodigoVacuno = celo.VacunoCodigo,
            NombreVacuno = celo.NombreVacuno,
            VecesEnCelo = vecesEnCeloCounts.GetValueOrDefault(celo.VacunoId, 1),
        };
    }
}
