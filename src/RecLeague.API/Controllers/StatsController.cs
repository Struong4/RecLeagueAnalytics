using Microsoft.AspNetCore.Mvc;
using RecLeague.Application.DTOs;
using RecLeague.Application.Interfaces;

namespace RecLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly IStatLineService _statLineService;

    public StatsController(IStatLineService statLineService)
    {
        _statLineService = statLineService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var statLines = await _statLineService.GetAllAsync();
        return Ok(statLines);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var statLine = await _statLineService.GetByIdAsync(id);
        if (statLine == null) return NotFound();
        return Ok(statLine);
    }

    [HttpGet("player/{playerId}")]
    public async Task<IActionResult> GetByPlayerId(int playerId)
    {
        var statLines = await _statLineService.GetByPlayerIdAsync(playerId);
        return Ok(statLines);
    }

    [HttpGet("game/{gameId}")]
    public async Task<IActionResult> GetByGameId(int gameId)
    {
        var statLines = await _statLineService.GetByGameIdAsync(gameId);
        return Ok(statLines);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateStatLineDto dto)
    {
        var created = await _statLineService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateStatLineDto dto)
    {
        await _statLineService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _statLineService.DeleteAsync(id);
        return NoContent();
    }
}
