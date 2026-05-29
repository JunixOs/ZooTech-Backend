using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;


[ApiController]
[Route("v1/produccion-leche/reporte")]
public class ReporteLecheController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "Api funcionando de manera correcta",
            model = "reporte produccion leche"
        });
    }

}
