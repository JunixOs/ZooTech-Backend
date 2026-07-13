using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Shared.Enums;

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

    public async Task<ExportarArbolGenealogicoOutput> HandleAsync(
        ExportarArbolGenealogicoCommand command, CancellationToken cancellationToken = default)
    {
        var vacunoRaiz = await _vacunoRepository.GetByIdAsync(command.VacunoId, cancellationToken);
        if (vacunoRaiz == null)
        {
            throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Vacuno,
                $"No se encontró el vacuno con ID {command.VacunoId}."
            );
        }
        var nodosArbol = await _vacunoRepository.GetArbolGenealogicoAsync(
            command.VacunoId, command.Niveles, cancellationToken);

        var excelBytes = await _exportService.GenerateExcelAsync(nodosArbol, vacunoRaiz, cancellationToken);

        return new ExportarArbolGenealogicoOutput(excelBytes);
    }
}