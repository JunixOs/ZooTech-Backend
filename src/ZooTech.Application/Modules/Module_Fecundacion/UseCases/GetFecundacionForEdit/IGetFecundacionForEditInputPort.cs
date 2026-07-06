namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;

public interface IGetFecundacionForEditInputPort
{
    Task<GetFecundacionForEditOutput> HandleAsync(long id, CancellationToken cancellationToken = default);
}
