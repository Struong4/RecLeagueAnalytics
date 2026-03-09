namespace RecLeague.Application.DTOs;

// Returned by GET /api/analytics/team/{id}
public class TeamAnalyticsDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;
    public int GamesPlayed { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public double WinPct { get; set; }

    // Per-game averages across all players on the team
    public double PointsPerGame { get; set; }
    public double ReboundsPerGame { get; set; }
    public double AssistsPerGame { get; set; }

    // Offensive efficiency = points scored per game
    // Defensive efficiency = points allowed per game
    // Net efficiency = offensive - defensive (positive means you outscore opponents on average)
    public double OffensiveEfficiency { get; set; }
    public double DefensiveEfficiency { get; set; }
    public double NetEfficiency { get; set; }
}

// Returned by GET /api/analytics/season/{year}/summary
public class SeasonSummaryDto
{
    public int Season { get; set; }
    public int TotalGames { get; set; }
    public int TotalTeams { get; set; }
    public int TotalPlayers { get; set; }
    public LeaderboardEntryDto? TopScorer { get; set; }
    public LeaderboardEntryDto? TopEfficiency { get; set; }
}
