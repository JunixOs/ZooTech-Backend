using ZooTech.Domain.Module_Celo.Entities;

namespace ZooTech.Domain.Module_Celo.Interfaces;

public interface ICeloDetalleRepository
{
    Task<CeloDetallePorVacuno> GetDetallePorVacunoAsync(
        string codigoVacuno,
        long registroId,
        CancellationToken cancellationToken = default);
}
