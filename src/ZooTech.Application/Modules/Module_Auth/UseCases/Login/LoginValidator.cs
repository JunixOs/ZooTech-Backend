using System.Text.RegularExpressions;
using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Auth.UseCases
{
    public class LoginValidator : ICommandValidator<LoginCommand>
    {
        public ModuleName ModuleName => ModuleName.Auth;

        public List<string> Validate(LoginCommand cmd)
        {
            List<string> errors = new List<string>();
            
            if(cmd.Password is null)
            {
                errors.Add("AUTH_LOGIN_PASSWORD_NULL");
            }
            else if (cmd.Password.Length <= 8 || cmd.Password.Length == 0)
            {
                errors.Add("AUTH_LOGIN_PASSWORD_LENGTH");
            }

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (cmd.Email is null)
            {
                errors.Add("AUTH_LOGIN_EMAIL_NULL");
            }
            else if (!Regex.IsMatch(cmd.Email , emailPattern))
            {
                errors.Add("AUTH_LOGIN_EMAIL_INVALID_FORMAT");
            }

            return errors;
        }
    }
}