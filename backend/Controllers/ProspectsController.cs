using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProspectsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProspectsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Prospect>>> GetAll([FromQuery] int? sourceId)
    {
        var query = _context.Prospects.Include(p => p.Source).AsQueryable();

        if (sourceId.HasValue)
        {
            query = query.Where(p => p.SourceId == sourceId.Value);
        }

        return await query.OrderBy(p => p.Rank).ToListAsync();
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Prospect>> GetById(int id)
    {
        var prospect = await _context.Prospects
            .Include(p => p.Source)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (prospect == null)
        {
            return NotFound();
        }

        return prospect;
    }

    [HttpPost]
    public async Task<ActionResult<Prospect>> Create([FromBody] Prospect prospect)
    {
        _context.Prospects.Add(prospect);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = prospect.Id }, prospect);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Prospect prospect)
    {
        if (id != prospect.Id)
        {
            return BadRequest();
        }

        _context.Entry(prospect).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Prospects.AnyAsync(p => p.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var prospect = await _context.Prospects.FindAsync(id);

        if (prospect == null)
        {
            return NotFound();
        }

        _context.Prospects.Remove(prospect);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
