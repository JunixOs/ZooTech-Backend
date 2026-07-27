using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.Validators
{
    public class CreateOrdenioValidator : ICommandQueryValidator<CreateOrdenioCommand>
    {
        public ModuleName ModuleName => ModuleName.Produccion_Leche;

        public List<string> Validate(CreateOrdenioCommand request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Codigo))
            {
                errors.Add("PRODUCCION_LECHE-ORDENIO-CREATE-CODIGO-NULL");
            }
            else if (request.Codigo.Length > 50)
            {
                errors.Add("PRODUCCION_LECHE-ORDENIO-CREATE-CODIGO-INVALID");
            }

            if (request.FechaHora == default)
            {
                errors.Add("PRODUCCION_LECHE-ORDENIO-CREATE-FECHA_HORA-NULL");
            }

            if (request.VacunoId <= 0)
            {
                errors.Add("PRODUCCION_LECHE-ORDENIO-CREATE-VACUNO_ID-INVALID");
            }

            if (request.EncargadoUsuarioId <= 0)
            {
                errors.Add("PRODUCCION_LECHE-ORDENIO-CREATE-ENCARGADO_USUARIO_ID-INVALID");
            }

            if (request.Litros <= 0)
            {
                errors.Add("PRODUCCION_LECHE-ORDENIO-CREATE-LITROS-INVALID");
            }

            if (string.IsNullOrWhiteSpace(request.EstadoOrdenioCode))
            {
                errors.Add("PRODUCCION_LECHE-ORDENIO-CREATE-ESTADO_ORDENIO_CODE-NULL");
            }

            if (!string.IsNullOrWhiteSpace(request.Observaciones) &&
                request.Observaciones.Length > 150)
            {
                errors.Add("PRODUCCION_LECHE-ORDENIO-CREATE-OBSERVACIONES-INVALID");
            }

            return errors;
        }
    }
}