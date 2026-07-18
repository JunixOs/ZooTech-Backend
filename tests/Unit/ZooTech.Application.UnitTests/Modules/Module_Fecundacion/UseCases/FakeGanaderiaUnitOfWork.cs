using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Fecundacion.UseCases;

internal sealed class FakeGanaderiaUnitOfWork : IGanaderiaUnitOfWork
{
    public FakeGanaderiaUnitOfWork(IFecundacionRepository fecundaciones)
    {
        Fecundaciones = fecundaciones;
    }

    public IVacunoRepository Vacunos => throw new NotSupportedException();

    public IFecundacionRepository Fecundaciones { get; }

    public IOrdenioRepository Ordenios => throw new NotSupportedException();

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default,
        Func<T, CancellationToken, Task<T>>? afterSave = null)
    {
        var result = await operation(cancellationToken);
        return afterSave is null ? result : await afterSave(result, cancellationToken);
    }
}
