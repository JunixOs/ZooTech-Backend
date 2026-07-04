using ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;

namespace ZooTech.Application.Modules.Module_Vacuno.Common;

public interface IVacunoQueryRepository
{
    Task<VacunoNodoDto?> GetArbolGenealogicoAsync(long vacunoId, int niveles);
    Task<VacunoDetalleDto?> GetDetalleByIdAsync(long id, CancellationToken cancellationToken = default);
}
