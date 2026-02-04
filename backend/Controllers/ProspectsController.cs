using backend.Data.Repositories;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProspectsController : ControllerBase
{
    private readonly IProspectRepository _prospectRepository;

    public ProspectsController(IProspectRepository prospectRepository)
    {
        _prospectRepository = prospectRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Prospect>>> GetAll([FromQuery] int? sourceId)
    {
        var prospects = await _prospectRepository.GetAllAsync(sourceId);
        return Ok(prospects);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Prospect>> GetById(int id)
    {
        var prospect = await _prospectRepository.GetByIdAsync(id);

        if (prospect == null)
        {
            return NotFound();
        }

        return prospect;
    }

    [HttpPost]
    public async Task<ActionResult<Prospect>> Create([FromBody] Prospect prospect)
    {
        await _prospectRepository.CreateAsync(prospect);

        return CreatedAtAction(nameof(GetById), new { id = prospect.Id }, prospect);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Prospect prospect)
    {
        if (id != prospect.Id)
        {
            return BadRequest();
        }

        if (!await _prospectRepository.ExistsAsync(id))
        {
            return NotFound();
        }

        await _prospectRepository.UpdateAsync(prospect);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _prospectRepository.DeleteAsync(id))
        {
            return NotFound();
        }

        return NoContent();
    }
}
