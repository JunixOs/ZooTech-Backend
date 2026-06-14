using ZooTech.Domain.Module_Vacuno.Entities;

namespace ZooTech.Application.Common.Gateway.Services;

public interface IArbolGenealogicoExportService
{
    Task<byte[]> GenerateExcelAsync(List<Vacuno> arbolGenealogico, Vacuno raiz, CancellationToken cancellationToken = default);
}
