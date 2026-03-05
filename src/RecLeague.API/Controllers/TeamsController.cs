using Microsoft.AspNetCore.Mvc;
using RecLeague.Application.DTOs;
using RecLeague.Application.Interfaces;

namespace RecLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]

// controller made to connect to service to eventually get data from sql
// GET /api/teams = all teams
// GET /api/teams/{id} = 1 team
// POST /api/teams = create team
// PUT /api/teams/{id} = update team
// DELETE /api/team/{id} = delete team
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var teams = await _teamService.GetAllAsync();
        return Ok(teams);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var team = await _teamService.GetByIdAsync(id);
        if (team == null) return NotFound();
        return Ok(team);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTeamDto dto)
    {
        var created = await _teamService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateTeamDto dto)
    {
        await _teamService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _teamService.DeleteAsync(id);
        return NoContent();
    }
}
