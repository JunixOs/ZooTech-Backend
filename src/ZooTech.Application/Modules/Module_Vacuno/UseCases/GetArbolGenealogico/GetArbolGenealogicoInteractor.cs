using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.GetArbolGenealogico;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;

public sealed class GetArbolGenealogicoInteractor : IGetArbolGenealogicoInputPort
{
    private readonly IVacunoRepository _vacunoRepository;

    public GetArbolGenealogicoInteractor(IVacunoRepository vacunoRepository)
    {
        _vacunoRepository = vacunoRepository;
    }
    public async Task<GetArbolGenealogicoOutput> HandleAsync(
    GetArbolGenealogicoCommand command, CancellationToken cancellationToken = default)
    {
        var nodos = await _vacunoRepository.GetArbolGenealogicoAsync(
            command.Id, command.Niveles, cancellationToken);

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