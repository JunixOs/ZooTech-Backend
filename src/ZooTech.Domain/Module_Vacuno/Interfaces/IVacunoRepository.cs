using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.ReadModels.ListarVacuno;
using ZooTech.Domain.Module_Vacuno.ReadModels.GetArbolGenealogico;

namespace ZooTech.Domain.Module_Vacuno.Interfaces;

public interface IVacunoRepository
{
    Task<List<Vacuno>> ListAllAsync(CancellationToken cancellationToken = default);

    Task<List<Vacuno>> ListAllWithDeletedAsync(CancellationToken cancellationToken = default);

    Task<List<(Vacuno Vacuno, string? Procedencia)>> ListAllForDisplayAsync(CancellationToken cancellationToken = default);

    Task<Vacuno?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<Vacuno?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);

    Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default);

    Task<Vacuno> AddAsync(Vacuno vacuno, CancellationToken cancellationToken = default);

    Task<Vacuno> UpdateAsync(Vacuno vacuno, CancellationToken cancellationToken = default);

    Task<(List<VacunoListItem> Items, int TotalCount)> GetPagedAsync(
        string? query, DateTime? fechaDesde, DateTime? fechaHasta, string? estado, int page, int limit, CancellationToken cancellationToken = default);

    Task<List<VacunoGenealogiaNode>> GetArbolGenealogicoAsync(
    long id, int maxNiveles, CancellationToken cancellationToken = default);
}
