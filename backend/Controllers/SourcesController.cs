using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SourcesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SourcesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Source>>> GetAll()
    {
        return await _context.Sources.OrderBy(s => s.Name).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Source>> Create([FromBody] Source source)
    {
        _context.Sources.Add(source);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = source.Id }, source);
    }
}
