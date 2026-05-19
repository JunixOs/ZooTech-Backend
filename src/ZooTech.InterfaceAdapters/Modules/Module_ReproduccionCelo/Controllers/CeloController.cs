using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.DTOs.Reproduccion;
using ZooTech.Infrastructure.Persistence.Repositories.Reproduccion;

namespace ZooTech.InterfaceAdapters.Modules.Module_ReproduccionCelo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CeloController : ControllerBase
    {
        private readonly CeloRepository _repository;

        public CeloController(CeloRepository repository)
        {
            _repository = repository;
        }

        [HttpPut("editar")]
        public async Task<IActionResult> EditarCelo(
            [FromBody] EditarCeloDTO dto)
        {
            var resultado = await _repository.EditarCeloAsync(dto);

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
