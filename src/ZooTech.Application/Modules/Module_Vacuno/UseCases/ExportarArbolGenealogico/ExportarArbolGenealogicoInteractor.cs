using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.Common;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;

public sealed class ExportarArbolGenealogicoInteractor : IExportarArbolGenealogicoInputPort
{
    private readonly IVacunoRepository _vacunoRepository;
    private readonly IReportStrategyResolver<GenealogiaVacunoReportModel> _strategyResolver;
    private readonly IVacunoReportFormatPolicy _formatPolicy;
    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;

    public ExportarArbolGenealogicoInteractor(
        IVacunoRepository vacunoRepository,
        IReportStrategyResolver<GenealogiaVacunoReportModel> strategyResolver,
        IVacunoReportFormatPolicy formatPolicy,
        ITenantConfigurationProvider tenantConfigurationProvider)
    {
        _vacunoRepository = vacunoRepository;
        _strategyResolver = strategyResolver;
        _formatPolicy = formatPolicy;
        _tenantConfigurationProvider = tenantConfigurationProvider;
    }

    public async Task<ExportarArbolGenealogicoOutput> HandleAsync(
        ExportarArbolGenealogicoQuery query, CancellationToken cancellationToken = default)
    {
        // 1. Verificar existencia del vacuno raíz
        var vacunoRaiz = await _vacunoRepository.GetByIdAsync(query.VacunoId, cancellationToken);
        if (vacunoRaiz == null)
        {
            throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Vacuno,
                $"No se encontró el vacuno con ID {query.VacunoId}."
            );
        }

        // 2. Capping elástico de niveles
        var minNiveles = await _tenantConfigurationProvider.GetSettingAsync(Settings.Vacunos.VacunosArbolMinNiveles);
        var maxNiveles = await _tenantConfigurationProvider.GetSettingAsync(Settings.Vacunos.VacunosArbolMaxNiveles);
        var nivelesAjustados = Math.Clamp(query.Niveles, minNiveles, maxNiveles);

        // 3. Consultar árbol
        var nodosArbol = await _vacunoRepository.GetArbolGenealogicoAsync(
            query.VacunoId, nivelesAjustados, cancellationToken);

        // 4. Validar política del tenant y generar con la estrategia solicitada
        var format = await _formatPolicy.EnsureAllowedAsync(query.Formato);
        var document = await _strategyResolver
            .Resolve(format)
            .GenerateAsync(
                new GenealogiaVacunoReportModel(nodosArbol, vacunoRaiz),
                cancellationToken);

        return new ExportarArbolGenealogicoOutput(
            document.Content,
            document.ContentType,
            document.FileName);
    }
}
