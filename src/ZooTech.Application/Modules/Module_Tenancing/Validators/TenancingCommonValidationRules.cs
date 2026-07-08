namespace ZooTech.Application.Modules.Module_Tenancing.Validators;

/// <summary>
/// Shared field-validation rules for Tenancing user-creation commands.
/// Both CreateAdminUser and CreateUserInTenant validators call this to avoid
/// duplicating identical Email/Username validation logic.
/// </summary>
internal static class TenancingCommonValidationRules
{
    public static void ValidateEmailAndUsername(
        List<string> errors,
        string operation,
        string? email,
        string? userName)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add($"TENANCING-ADMIN_USER-{operation}-EMAIL-NULL");
        }
        else if (!System.Net.Mail.MailAddress.TryCreate(email, out _))
        {
            errors.Add($"TENANCING-ADMIN_USER-{operation}-EMAIL-INVALID");
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            errors.Add($"TENANCING-ADMIN_USER-{operation}-USERNAME-NULL");
        }
        else if (userName.Length < 3 || userName.Length > 50)
        {
            errors.Add($"TENANCING-ADMIN_USER-{operation}-USERNAME-INVALID");
        }
    }
}
