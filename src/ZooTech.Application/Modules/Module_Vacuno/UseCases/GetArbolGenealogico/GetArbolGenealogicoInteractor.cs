using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Domain.Module_Vacuno.ReadModels.GetArbolGenealogico;

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
        var vacuno = await _vacunoRepository.GetByIdAsync(command.Id, cancellationToken);
        if (vacuno == null)
        {
            throw new VacunoNotFoundException($"No existe el vacuno con el ID {command.Id}.");
        }

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