using RecLeague.Application.DTOs;

namespace RecLeague.Application.Interfaces;

public interface IAnalyticsService
{
    Task<PlayerAnalyticsDto?> GetPlayerAnalyticsAsync(int playerId);
    Task<List<PlayerTrendDto>> GetPlayerTrendsAsync(int playerId);
    Task<TeamAnalyticsDto?> GetTeamAnalyticsAsync(int teamId);
    Task<List<LeaderboardEntryDto>> GetScoringLeaderboardAsync();
    Task<List<LeaderboardEntryDto>> GetEfficiencyLeaderboardAsync();
    Task<SeasonSummaryDto?> GetSeasonSummaryAsync(int year);
}
