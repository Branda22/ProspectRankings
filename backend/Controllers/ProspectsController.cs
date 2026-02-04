using backend.Data.Repositories;
using backend.Models;
using backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProspectsController : ControllerBase
{
    private readonly IProspectRepository _prospectRepository;
<<<<<<< HEAD
    private readonly IRankingRepository _rankingRepository;

    public ProspectsController(IProspectRepository prospectRepository, IRankingRepository rankingRepository)
    {
        _prospectRepository = prospectRepository;
        _rankingRepository = rankingRepository;
=======

    public ProspectsController(IProspectRepository prospectRepository)
    {
        _prospectRepository = prospectRepository;
>>>>>>> 610df4e (fixes deployment script)
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Ranking>>> GetAll()
    {
<<<<<<< HEAD
        var prospectRankings = await _rankingRepository.GetAllAsync();
        return Ok(prospectRankings);
=======
        var prospects = await _prospectRepository.GetAllAsync(sourceId);
        return Ok(prospects);
>>>>>>> 610df4e (fixes deployment script)
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Prospect>> GetById(Guid id)
    {
        var prospect = await _prospectRepository.GetByIdAsync(id);

        if (prospect == null)
        {
            return NotFound();
        }

        return prospect;
    }

    [HttpPost]
    public async Task<ActionResult<List<Guid>>> Create([FromBody] ProspectListDto prospectList)
    {
<<<<<<< HEAD
        var list = prospectList.List.Select(p => new Prospect
        {
            PlayerName = p.PlayerName,
            Source = prospectList.Source,
            Age = p.Age,
            Position = p.Position,
            Team = p.Team,
            ETA = p.ETA,
            Rank = p.Rank
        });
        var ids = await _prospectRepository.BulkCreateAsync(list);
=======
        await _prospectRepository.CreateAsync(prospect);
>>>>>>> 610df4e (fixes deployment script)

        return Created("api/prospects", ids);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Prospect prospect)
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
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await _prospectRepository.DeleteAsync(id))
        {
            return NotFound();
        }

        return NoContent();
    }
}
