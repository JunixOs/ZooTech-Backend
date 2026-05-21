using ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;
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
}
