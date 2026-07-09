namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;

public interface IGetFecundacionForEditInputPort
{
    Task<GetFecundacionForEditOutput> HandleAsync(GetFecundacionForEditCommand cmd, CancellationToken cancellationToken = default);
}
