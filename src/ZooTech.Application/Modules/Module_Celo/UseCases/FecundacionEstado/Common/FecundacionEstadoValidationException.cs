namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;

public sealed class FecundacionEstadoValidationException : Exception
{
    public FecundacionEstadoValidationException(IReadOnlyDictionary<string, string> errors)
        : base("La solicitud de estado de fecundacion contiene errores de validacion.")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string> Errors { get; }
}
