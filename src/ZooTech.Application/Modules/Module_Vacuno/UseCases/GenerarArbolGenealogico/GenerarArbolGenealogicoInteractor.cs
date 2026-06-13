
using ZooTech.Application.Common.Configuration;
using ZooTech.Domain.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;

public sealed class GenerarArbolGenealogicoInteractor : IGenerarArbolGenealogicoInputPort
{
    private readonly IVacunoRepository _vacunoRepository;
    private readonly IVacunosConfiguration _settings;

    public GenerarArbolGenealogicoInteractor(IVacunoRepository vacunoRepository, IVacunosConfiguration settings)
    {
        _vacunoRepository = vacunoRepository;
        _settings = settings;
    }

    public async Task<GenerarArbolGenealogicoOutput> HandleAsync(long vacunoId, GenerarArbolGenealogicoCommand command, CancellationToken cancellationToken = default)
    {
        if (!await _vacunoRepository.ExistsAsync(vacunoId, cancellationToken))
        {
            throw new ArgumentException($"No se encontró el vacuno con ID {vacunoId}");
        }

        var minNiveles = _settings.ArbolMinNiveles;
        var maxNiveles = _settings.ArbolMaxNiveles;

        var niveles = command.Niveles;
        if (niveles < minNiveles) niveles = minNiveles;
        if (niveles > maxNiveles) niveles = maxNiveles;

        var vacunos = await _vacunoRepository.GetArbolGenealogicoAsync(vacunoId, niveles, cancellationToken);

        var dtos = vacunos.Select(x => new ArbolVacunoDto(
            Id: x.Id,
            Codigo: x.Codigo,
            Nombre: x.Nombre,
            SexoCode: x.SexoCode,
            PadreId: x.PadreId,
            MadreId: x.MadreId)).ToList();

        return new GenerarArbolGenealogicoOutput(dtos);
    }
}
