using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Domain.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;

public sealed class ExportarArbolGenealogicoInteractor : IExportarArbolGenealogicoInputPort
{
    private readonly IVacunoRepository _vacunoRepository;
    private readonly IArbolGenealogicoExportService _exportService;

    public ExportarArbolGenealogicoInteractor(
        IVacunoRepository vacunoRepository,
        IArbolGenealogicoExportService exportService)
    {
        _vacunoRepository = vacunoRepository;
        _exportService = exportService;
    }

    public async Task<byte[]> HandleAsync(
        long vacunoId, ExportarArbolGenealogicoCommand command, CancellationToken cancellationToken = default)
    {
        var vacunoRaiz = await _vacunoRepository.GetByIdAsync(vacunoId, cancellationToken);
        if (vacunoRaiz == null)
        {
            throw new NotFoundException($"No se encontró el vacuno con ID {vacunoId}.");
        }
        var nodosArbol = await _vacunoRepository.GetArbolGenealogicoAsync(
            vacunoId, command.Niveles, cancellationToken);

        return await _exportService.GenerateExcelAsync(nodosArbol, vacunoRaiz, cancellationToken);
    }
}