using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Vacunos.UseCases.ListarVacunos;
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

    Task<ZooTech.Application.Modules.Module_Vacunos.UseCases.GenerarArbolGenealogico.VacunoNodoDto?> GetArbolGenealogicoAsync(long vacunoId, int niveles);
}
