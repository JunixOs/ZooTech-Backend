using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Tenancing.Validators;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser
{
    public class CreateAdminUserValidator : ICommandQueryValidator<CreateAdminUserCommand>
    {
        public ModuleName ModuleName => ModuleName.Tenancing;

        public List<string> Validate(CreateAdminUserCommand request)
        {
            var errors = new List<string>();

            TenancingCommonValidationRules.ValidateEmailAndUsername(errors, "CREATE", request.Email, request.UserName);

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                errors.Add("TENANCING-ADMIN_USER-CREATE-PASSWORD-NULL");
            }
            else if (request.Password.Length < 8)
            {
                errors.Add("TENANCING-ADMIN_USER-CREATE-PASSWORD-INVALID");
            }

            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                errors.Add("TENANCING-ADMIN_USER-CREATE-FIRST_NAME-NULL");
            }

            return errors;
        }
    }
}