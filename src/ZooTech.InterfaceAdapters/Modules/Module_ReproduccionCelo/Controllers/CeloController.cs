using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.DTOs.Reproduccion;
using ZooTech.Infrastructure.Persistence.Repositories;

namespace ZooTech.InterfaceAdapters.Modules.Module_ReproduccionCelo.Controllers
{
    [ApiController]
    [Route("api/v1/celo")]
    public class CeloController : ControllerBase
    {
        private readonly CeloRepository _repository;

        public CeloController(CeloRepository repository)
        {
            _repository = repository;
        }

        /*
        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarCelo(
            [FromBody] RegistrarCeloDTO dto)
        {
            var resultado = await _repository.RegistrarCeloAsync(dto);

            if (!resultado)
            {
                return BadRequest(new
                {
                    mensaje = "No se pudo registrar el celo"
                });
            }

            return Ok(new
            {
                mensaje = "¡Celo registrado con éxito!"
            });
        }
        */

        [HttpPut("editar")]
        public async Task<IActionResult> EditarCelo(
            [FromBody] EditarCeloDTO dto)
        {
            var resultado = await _repository.EditarCeloAsync(
                dto.Id,
                dto.Observaciones,
                dto.CaracteristicaCodes);

            if (!resultado)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el registro de celo"
                });
            }

            return Ok(new
            {
                mensaje = "¡Celo editado con éxito!"
            });
        }
    }
}