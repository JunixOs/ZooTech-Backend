using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo
{
    public class CreateCeloValidator : ICommandValidator<CreateCeloCommand>
    {
        public ModuleName ModuleName => ModuleName.Celo;

        public List<string> Validate(CreateCeloCommand request)
        {
            var errors = new List<string>();

            if (request.VacunoId <= 0)
            {
                errors.Add("CELO-CELO-CREATE-VACUNO_ID-INVALID");
            }

            if (request.EncargadoUsuarioId <= 0)
            {
                errors.Add("CELO-CELO-CREATE-ENCARGADO_USUARIO_ID-INVALID");
            }

            if (request.FechaHora == default || request.FechaHora is null)
            {
                errors.Add("CELO-CELO-CREATE-FECHA_HORA-NULL");
            }

            if (!string.IsNullOrEmpty(request.Observaciones) &&
                request.Observaciones.Length > 500)
            {
                errors.Add("CELO-CELO-CREATE-OBSERVACIONES-INVALID");
            }

            if (request.CaracteristicaCodes is null ||
                request.CaracteristicaCodes.Count == 0)
            {
                errors.Add("CELO-CELO-CREATE-CARACTERISTICAS-NULL");
            }

            return errors;
        }
    }
}