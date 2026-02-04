using backend.Data.Repositories;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SourcesController : ControllerBase
{
    private readonly ISourceRepository _sourceRepository;

    public SourcesController(ISourceRepository sourceRepository)
    {
        _sourceRepository = sourceRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Source>>> GetAll()
    {
        var sources = await _sourceRepository.GetAllAsync();
        return Ok(sources);
    }

    [HttpPost]
    public async Task<ActionResult<Source>> Create([FromBody] Source source)
    {
        await _sourceRepository.CreateAsync(source);

        return CreatedAtAction(nameof(GetAll), new { id = source.Id }, source);
    }
}
