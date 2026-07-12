using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.ListarVacuno;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.GetArbolGenealogico;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Models;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;

namespace ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

public interface IVacunoRepository
{
    Task<List<Vacuno>> ListAllAsync(CancellationToken cancellationToken = default);

    Task<List<Vacuno>> ListAllWithDeletedAsync(CancellationToken cancellationToken = default);

    Task<List<(Vacuno Vacuno, string? Procedencia)>> ListAllForDisplayAsync(CancellationToken cancellationToken = default);

    Task<List<VacunoReferenceItem>> ListReferencesAsync(CancellationToken cancellationToken = default);

    Task<Vacuno?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<Vacuno?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);

    Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default);

    Task<VacunoCatalogs> GetCatalogsAsync(CancellationToken cancellationToken = default);

    Task<Vacuno> AddAsync(Vacuno vacuno, decimal? precioCompra, string? aptoPara, CancellationToken cancellationToken = default);

    Task<Vacuno> UpdateAsync(Vacuno vacuno, decimal? precioCompra, string? aptoPara, CancellationToken cancellationToken = default);

    Task<(List<VacunoListItem> Items, int TotalCount)> GetPagedAsync(
        string? query, DateTime? fechaDesde, DateTime? fechaHasta, string? estado, int page, int limit, CancellationToken cancellationToken = default);

    Task<List<VacunoGenealogiaNode>> GetArbolGenealogicoAsync(
        long id, int maxNiveles, CancellationToken cancellationToken = default);
}
