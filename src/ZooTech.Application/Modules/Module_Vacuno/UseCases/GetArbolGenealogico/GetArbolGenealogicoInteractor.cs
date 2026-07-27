using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.GetArbolGenealogico;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;

public sealed class GetArbolGenealogicoInteractor : IGetArbolGenealogicoInputPort
{
    private readonly IVacunoRepository _vacunoRepository;
    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;

    public GetArbolGenealogicoInteractor(
        IVacunoRepository vacunoRepository,
        ITenantConfigurationProvider tenantConfigurationProvider)
    {
        _vacunoRepository = vacunoRepository;
        _tenantConfigurationProvider = tenantConfigurationProvider;
    }

    public async Task<GetArbolGenealogicoOutput> HandleAsync(
        GetArbolGenealogicoQuery query, CancellationToken cancellationToken = default)
    {
        // 1. Validar que el vacuno raíz exista y no esté eliminado
        var vacunoRaiz = await _vacunoRepository.GetByIdAsync(query.Id, cancellationToken);
        if (vacunoRaiz == null)
        {
            throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Vacuno,
                $"No se encontró el vacuno con ID {query.Id}."
            );
        }

        // 2. Obtener límites dinámicos de niveles por Tenant
        var minNiveles = await _tenantConfigurationProvider.GetSettingAsync(Settings.Vacunos.VacunosArbolMinNiveles);
        var maxNiveles = await _tenantConfigurationProvider.GetSettingAsync(Settings.Vacunos.VacunosArbolMaxNiveles);

        // 3. Capping elástico de niveles según límites del Tenant
        var nivelesAjustados = Math.Clamp(query.Niveles, minNiveles, maxNiveles);

        // 4. Obtener árbol de genealogía
        var nodos = await _vacunoRepository.GetArbolGenealogicoAsync(
            query.Id, nivelesAjustados, cancellationToken);

        var items = nodos.Select(n => new GetArbolGenealogicoItem(
            Id: n.Vacuno.Id,
            Codigo: n.Vacuno.Codigo,
            Nombre: n.Vacuno.Nombre,
            FechaNacimiento: n.Vacuno.FechaNacimiento,
            RazaCode: n.Vacuno.RazaCode,
            Procedencia: n.Procedencia,
            PadreId: n.Vacuno.PadreId,
            MadreId: n.Vacuno.MadreId,
            Nivel: n.Nivel
        )).ToList();

        return new GetArbolGenealogicoOutput(items);
    }
}