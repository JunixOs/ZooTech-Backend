namespace ZooTech.Domain.Common.Interfaces;

public interface IEstadoRegistroRepository
{
    Task<string> GetActiveCodeAsync(CancellationToken cancellationToken = default);
}
