using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser
{
    public class CreateAdminUserValidator : ICommandValidator<CreateAdminUserCommand>
    {
        public ModuleName ModuleName => ModuleName.Tenancing;

        public List<string> Validate(CreateAdminUserCommand request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errors.Add("TENANCING-ADMIN_USER-CREATE-EMAIL-NULL");
            }
            else if (!System.Net.Mail.MailAddress.TryCreate(request.Email, out _))
            {
                errors.Add("TENANCING-ADMIN_USER-CREATE-");
            }

            if (string.IsNullOrWhiteSpace(request.UserName))
            {
                errors.Add("TENANCING-ADMIN_USER-CREATE-USERNAME-NULL");
            }
            else if (request.UserName.Length < 3 || request.UserName.Length > 50)
            {
                errors.Add("TENANCING-ADMIN_USER-CREATE-USERNAME-INVALID");
            }

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