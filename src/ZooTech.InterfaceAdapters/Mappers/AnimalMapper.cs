using ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;
using ZooTech.InterfaceAdapters.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Mappers;

public static class AnimalMapper
{
    public static DeleteAnimalResponse ToDeleteResponse(DeleteAnimalOutput output)
    {
        return new DeleteAnimalResponse
        {
            Id = output.Id,
            Codigo = output.Codigo,
            Nombre = output.Nombre,
            MotivoEliminacion = output.MotivoEliminacion,
            EliminadoPor = output.EliminadoPor,
            FechaEliminacion = output.FechaEliminacion,
            ResueltoComoDuplicado = output.ResueltoComoDuplicado
        };
    }

    public static ReportAnimalListResponse ToReportAnimalListResponse(ReportAnimalListOutput output)
    {
        return new ReportAnimalListResponse
        {
            FechaInicio = output.FechaInicio,
            FechaFin = output.FechaFin,
            Keyword = output.Keyword,
            Total = output.Items.Count,
            Items = output.Items.Select(ToReportAnimalListItemResponse).ToArray()
        };
    }

    private static ReportAnimalListItemResponse ToReportAnimalListItemResponse(ReportAnimalListItem item)
    {
        return new ReportAnimalListItemResponse
        {
            Codigo = item.Codigo,
            Nombre = item.Nombre,
            Raza = item.Raza,
            Sexo = item.Sexo,
            Procedencia = item.Procedencia,
            Estado = item.Estado,
            FechaRegistro = item.FechaRegistro
        };
    }
}
