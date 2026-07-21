using System;
using System.Threading;
using System.Threading.Tasks;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;

public sealed class ExportarArbolGenealogicoInteractor : IExportarArbolGenealogicoInputPort
{
    private readonly IVacunoRepository _vacunoRepository;
    private readonly IArbolGenealogicoExportService _exportService;
    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;

    public ExportarArbolGenealogicoInteractor(
        IVacunoRepository vacunoRepository,
        IArbolGenealogicoExportService exportService,
        ITenantConfigurationProvider tenantConfigurationProvider)
    {
        _vacunoRepository = vacunoRepository;
        _exportService = exportService;
        _tenantConfigurationProvider = tenantConfigurationProvider;
    }

    public async Task<ExportarArbolGenealogicoOutput> HandleAsync(
        ExportarArbolGenealogicoCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Verificar existencia del vacuno raíz
        var vacunoRaiz = await _vacunoRepository.GetByIdAsync(command.VacunoId, cancellationToken);
        if (vacunoRaiz == null)
        {
            throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Vacuno,
                $"No se encontró el vacuno con ID {command.VacunoId}."
            );
        }

        // 2. Capping elástico de niveles
        var minNiveles = await _tenantConfigurationProvider.GetSettingAsync(Settings.Vacunos.VacunosArbolMinNiveles);
        var maxNiveles = await _tenantConfigurationProvider.GetSettingAsync(Settings.Vacunos.VacunosArbolMaxNiveles);
        var nivelesAjustados = Math.Clamp(command.Niveles, minNiveles, maxNiveles);

        // 3. Consultar árbol
        var nodosArbol = await _vacunoRepository.GetArbolGenealogicoAsync(
            command.VacunoId, nivelesAjustados, cancellationToken);

        // 4. Generar reporte según el formato
        byte[] fileBytes;
        string contentType;
        string extension;

        if (command.Formato?.ToLowerInvariant() == "pdf")
        {
            fileBytes = await _exportService.GeneratePdfAsync(nodosArbol, vacunoRaiz, cancellationToken);
            contentType = "application/pdf";
            extension = "pdf";
        }
        else
        {
            fileBytes = await _exportService.GenerateExcelAsync(nodosArbol, vacunoRaiz, cancellationToken);
            contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            extension = "xlsx";
        }

        var fileName = $"Genealogia_{vacunoRaiz.Codigo}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{extension}";

        return new ExportarArbolGenealogicoOutput(fileBytes, contentType, fileName);
    }
}