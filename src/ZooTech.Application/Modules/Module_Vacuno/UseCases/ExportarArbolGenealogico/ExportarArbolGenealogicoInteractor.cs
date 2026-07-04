using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.ReadModels.GetArbolGenealogico;

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
        // 1. Obtener el árbol jerárquico recursivo usando la consulta CTE local optimizada
        var arbolHierarchical = await _vacunoRepository.GetArbolGenealogicoAsync(vacunoId, command.Niveles);
        if (arbolHierarchical == null)
        {
            throw new VacunoNotFoundException($"No se encontró el vacuno con ID {vacunoId}.");
        }

        // 2. Aplanar la estructura jerárquica a una lista plana de VacunoGenealogiaNode compatible con ClosedXML
        var flatList = new List<VacunoGenealogiaNode>();
        Flatten(arbolHierarchical, 1, flatList);

        // 3. Obtener el vacuno raíz del listado
        var raiz = flatList.FirstOrDefault(n => n.Vacuno.Id == vacunoId)?.Vacuno;
        if (raiz == null)
        {
            throw new VacunoNotFoundException($"No se encontró la raíz del árbol para el vacuno con ID {vacunoId}.");
        }

        // 4. Delegar la exportación al servicio ClosedXML
        return await _exportService.GenerateExcelAsync(flatList, raiz, cancellationToken);
    }

    private void Flatten(VacunoNodoDto? node, int nivel, List<VacunoGenealogiaNode> list)
    {
        if (node == null) return;

        // Rehidratar un modelo de dominio Vacuno compatible con los campos que el exportador a Excel necesita
        var domainVacuno = Vacuno.Rehydrate(
            id: node.Id,
            codigo: node.Codigo,
            nombre: node.Nombre,
            fechaNacimiento: DateOnly.MinValue,
            tipoAdquisicionCode: "COMPRA",
            razaCode: node.Raza,
            colorCode: "NEGRO",
            sexoCode: node.Sexo,
            padreId: node.Padre?.Id,
            madreId: node.Madre?.Id,
            granjaId: 1,
            observaciones: null,
            fechaRegistro: DateOnly.MinValue,
            createdAt: DateTime.UtcNow,
            updatedAt: DateTime.UtcNow,
            deletedAt: null,
            motivoEliminacion: null,
            createdBy: 1,
            updatedBy: 1,
            deletedBy: null
        );

        // Evitar duplicidades si en algún punto del árbol vuelve a aparecer el mismo ID
        if (!list.Any(x => x.Vacuno.Id == node.Id))
        {
            list.Add(new VacunoGenealogiaNode(domainVacuno, nivel, null));
        }

        if (node.Padre != null) Flatten(node.Padre, nivel + 1, list);
        if (node.Madre != null) Flatten(node.Madre, nivel + 1, list);
    }
}
