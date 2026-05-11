using Microsoft.AspNetCore.Mvc;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.InterfaceAdapters.Module_Sanidad.Controllers;

[ApiController]
[Route("api/v1/triaje")]
public class TriajeController : ControllerBase
{
    private readonly ITriajeRepository _triajeRepository;

    public TriajeController(ITriajeRepository triajeRepository)
    {
        _triajeRepository = triajeRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] object triaje)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] object triaje)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        throw new NotImplementedException();
    }
}