using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.GetArbolGenealogico;

namespace ZooTech.Application.Common.Gateway.Services;

public interface IArbolGenealogicoExportService
{
    Task<byte[]> GenerateExcelAsync(
        List<VacunoGenealogiaNode> arbolGenealogico, Vacuno raiz, CancellationToken cancellationToken = default);
}