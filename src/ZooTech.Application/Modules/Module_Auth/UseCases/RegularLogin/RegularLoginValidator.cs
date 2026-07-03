using System.Text.RegularExpressions;
using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Auth.UseCases.RegularLogin
{
    public class RegularLoginValidator : ICommandValidator<string>
    {
        public ModuleName ModuleName => ModuleName.Auth;

        public List<string> Validate(string email)
        {
            List<string> errors = new List<string>();

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (email is null)
            {
                errors.Add("AUTH_LOGIN_EMAIL_NULL");
            }
            else if (!Regex.IsMatch(email , emailPattern))
            {
                errors.Add("AUTH_LOGIN_EMAIL_INVALID_FORMAT");
            }

            return errors;
        }
    }
}