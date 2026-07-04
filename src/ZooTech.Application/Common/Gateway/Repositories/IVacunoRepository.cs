using ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunosPaginado;
using ZooTech.Domain.Enums;

namespace ZooTech.Application.Common.Gateway.Repositories;

public interface IVacunoRepository
{
    Task<(List<VacunoResumen> Data, int Total)> GetPagedAsync(
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        EstadoAnimal? estado,
        string? q,
        int skip,
        int take);

    Task<VacunoNodoDto?> GetArbolGenealogicoAsync(long vacunoId, int niveles);
}
