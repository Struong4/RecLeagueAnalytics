using Microsoft.AspNetCore.Mvc;
using RecLeague.Application.DTOs;
using RecLeague.Application.Interfaces;

namespace RecLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var players = await _playerService.GetAllAsync();
        return Ok(players);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var player = await _playerService.GetByIdAsync(id);
        if (player == null) return NotFound();
        return Ok(player);
    }

    [HttpGet("team/{teamId}")]
    public async Task<IActionResult> GetByTeamId(int teamId)
    {
        var players = await _playerService.GetByTeamIdAsync(teamId);
        return Ok(players);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePlayerDto dto)
    {
        var created = await _playerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreatePlayerDto dto)
    {
        await _playerService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _playerService.DeleteAsync(id);
        return NoContent();
    }
}
