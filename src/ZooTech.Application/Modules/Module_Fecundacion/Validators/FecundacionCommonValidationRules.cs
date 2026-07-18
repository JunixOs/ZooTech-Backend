using ZooTech.Domain.Ganaderia.Module_Fecundacion.Rules;

namespace ZooTech.Application.Modules.Module_Fecundacion.Validators;

/// <summary>
/// Shared field-validation rules for Fecundacion Create/Update commands.
/// Both validators call this to avoid duplicating identical CodigoSemen/CodigoEmbrion
/// validation logic; only the error-code prefix (CREATE/UPDATE) differs per operation.
/// </summary>
internal static class FecundacionCommonValidationRules
{
    public static void ValidateCodigoSemenAndEmbrion(
        List<string> errors,
        string operation,
        string tipoFecundacionCode,
        string? codigoSemen,
        string? codigoEmbrion)
    {
        if (FecundacionRules.EsInseminacionArtificial(tipoFecundacionCode))
        {
            if (string.IsNullOrWhiteSpace(codigoSemen))
            {
                errors.Add($"FECUNDACION-FECUNDACION-{operation}-CODIGO_SEMEN-NULL");
            }
            else if (codigoSemen.Length > 30)
            {
                errors.Add($"FECUNDACION-FECUNDACION-{operation}-CODIGO_SEMEN-INVALID");
            }
        }

        if (FecundacionRules.EsTransferenciaEmbriones(tipoFecundacionCode))
        {
            if (string.IsNullOrWhiteSpace(codigoEmbrion))
            {
                errors.Add($"FECUNDACION-FECUNDACION-{operation}-CODIGO_EMBRION-NULL");
            }
            else if (codigoEmbrion.Length > 30)
            {
                errors.Add($"FECUNDACION-FECUNDACION-{operation}-CODIGO_EMBRION-INVALID");
            }
        }
    }
}
