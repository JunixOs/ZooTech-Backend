using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using ZooTech.InterfaceAdapters.DTOs;

namespace ZooTech.InterfaceAdapters.Controllers;

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
