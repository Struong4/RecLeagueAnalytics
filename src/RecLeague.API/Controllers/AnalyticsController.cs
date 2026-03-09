using Microsoft.AspNetCore.Mvc;
using RecLeague.Application.Interfaces;

namespace RecLeague.API.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("player/{id}")]
    public async Task<IActionResult> GetPlayerAnalytics(int id)
    {
        var result = await _analyticsService.GetPlayerAnalyticsAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("player/{id}/trends")]
    public async Task<IActionResult> GetPlayerTrends(int id)
    {
        var result = await _analyticsService.GetPlayerTrendsAsync(id);
        return Ok(result);
    }

    [HttpGet("team/{id}")]
    public async Task<IActionResult> GetTeamAnalytics(int id)
    {
        var result = await _analyticsService.GetTeamAnalyticsAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("leaderboards/scoring")]
    public async Task<IActionResult> GetScoringLeaderboard()
    {
        var result = await _analyticsService.GetScoringLeaderboardAsync();
        return Ok(result);
    }

    [HttpGet("leaderboards/efficiency")]
    public async Task<IActionResult> GetEfficiencyLeaderboard()
    {
        var result = await _analyticsService.GetEfficiencyLeaderboardAsync();
        return Ok(result);
    }

    [HttpGet("season/{year}/summary")]
    public async Task<IActionResult> GetSeasonSummary(int year)
    {
        var result = await _analyticsService.GetSeasonSummaryAsync(year);
        return result == null ? NotFound() : Ok(result);
    }
}
