using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Features;
using ZooTech.Application.Common.Gateway.Repositories;

namespace ZooTech.Application.Modules.Module_Vacunos.UseCases.GenerarArbolGenealogico;

public class GenerarArbolGenealogicoInteractor : IGenerarArbolGenealogicoInputPort
{
    private readonly IVacunoRepository _repository;
    private readonly IGenerarArbolGenealogicoOutputPort _output;
    private readonly IFeatureService _features;
    private readonly ITenantContext _tenant;

    public GenerarArbolGenealogicoInteractor(
        IVacunoRepository repository,
        IGenerarArbolGenealogicoOutputPort output,
        IFeatureService features,
        ITenantContext tenant)
    {
        _repository = repository;
        _output = output;
        _features = features;
        _tenant = tenant;
    }

    public async Task Handle(GenerarArbolGenealogicoCommand command)
    {
        
        if (!await _features.IsEnabledAsync("module.vacunos"))
        {
            await _output.Error("FEATURE_DISABLED", "El módulo de vacunos no esta habilitado para este tenant.");
            return;
        }

       
        int niveles = command.Niveles;
        if (niveles < 1) niveles = 1;
        if (niveles > 4) niveles = 4;

        
        var arbol = await _repository.GetArbolGenealogicoAsync(command.VacunoId, niveles);

        if (arbol == null)
        {
            await _output.NotFound($"No se encontro el vacuno con ID {command.VacunoId} o fue eliminado.");
            return;
        }

        
        await _output.Ok(new GenerarArbolGenealogicoOutput
        {
            Data = arbol
        });
    }
}
