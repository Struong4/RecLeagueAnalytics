using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RecLeague.Application.DTOs;
using RecLeague.Application.Exceptions;
using RecLeague.Application.Interfaces;

namespace RecLeague.API.Controllers;

[ApiController]
[Route("api/ingestion")]
public class IngestionController : ControllerBase
{
    private readonly IIngestionService _ingestionService;
    private readonly IValidator<IngestionRequestDto> _validator;

    public IngestionController(IIngestionService ingestionService, IValidator<IngestionRequestDto> validator)
    {
        _ingestionService = ingestionService;
        _validator = validator;
    }

    [HttpPost("game")]
    public async Task<IActionResult> IngestGame([FromBody] IngestionRequestDto dto)
    {
        // Validate the payload before touching the database
        var result = await _validator.ValidateAsync(dto);
        if (!result.IsValid)
            return BadRequest(result.Errors.Select(e => e.ErrorMessage));

        try
        {
            var gameId = await _ingestionService.IngestGameAsync(dto);
            return CreatedAtAction(nameof(IngestGame), new { id = gameId }, new { gameId });
        }
        catch (DuplicateGameException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}
