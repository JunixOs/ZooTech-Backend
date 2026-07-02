using Microsoft.AspNetCore.Mvc;
using ZooTech.InterfaceAdapters.Middleware;

namespace ZooTech.InterfaceAdapters.Modules.Module_Auth
{
    [ApiController]
    [Route("auth")]
    [ApiExplorerSettings(GroupName = "auth")]
    public class AuthController : ControllerBase
    {
        public AuthController()
        {
            
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            
        }
    }
}