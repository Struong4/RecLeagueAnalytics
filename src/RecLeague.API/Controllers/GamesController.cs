using Microsoft.AspNetCore.Mvc;
using RecLeague.Application.DTOs;
using RecLeague.Application.Interfaces;

namespace RecLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var games = await _gameService.GetAllAsync();
        return Ok(games);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var game = await _gameService.GetByIdAsync(id);
        if (game == null) return NotFound();
        return Ok(game);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateGameDto dto)
    {
        var created = await _gameService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateGameDto dto)
    {
        await _gameService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _gameService.DeleteAsync(id);
        return NoContent();
    }
}
