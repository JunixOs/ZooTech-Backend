using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Models;

namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers
{
    [ApiController]
    [Route("api/v1/home")]
    [ApiExplorerSettings(GroupName = "public")]
    public class HomeController : ControllerBase
    {
        [HttpGet("health")]
        [Tags("Health")]
        public IActionResult Health()
        {
            return StatusCode(200, GeneralResponseDTO<string>.Ok("Swagger funcionando correctamente"));
        }

        [HttpGet]
        [Tags("Home")]
        public IActionResult Home()
        {
            return StatusCode(200, GeneralResponseDTO<string>.Ok("Bienvenido a la app multitenant de ZooTech"));
        }
    }
}
