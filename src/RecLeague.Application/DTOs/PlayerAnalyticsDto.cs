namespace RecLeague.Application.DTOs;

// Returned by GET /api/analytics/player/{id}
// All percentage/ratio fields are nullable — if a player has no attempts they get null instead of a bad number
public class PlayerAnalyticsDto
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public int GamesPlayed { get; set; }

    // Per-game averages
    public double PointsPerGame { get; set; }
    public double ReboundsPerGame { get; set; }
    public double AssistsPerGame { get; set; }
    public double StealsPerGame { get; set; }
    public double BlocksPerGame { get; set; }

    // Shooting efficiency
    public double? TrueShootingPct { get; set; }   // PTS / (2 * (FGA + 0.44 * FTA))
    public double? FieldGoalPct { get; set; }       // FGM / FGA
    public double? ThreePointPct { get; set; }      // 3PM / 3PA
    public double? FreeThrowPct { get; set; }       // FTM / FTA

    // Advanced
    public double? AstToRatio { get; set; }         // AST / TOV
    public double? Per36 { get; set; }              // Simplified PER scaled to 36 minutes
    public double? UsageRate { get; set; }          // Player's share of team possessions
}

// One data point in a player's game-by-game trend — returned by GET /api/analytics/player/{id}/trends
public class PlayerTrendDto
{
    public int GameId { get; set; }
    public DateTime GameDate { get; set; }
    public int Points { get; set; }
    public double? TrueShootingPct { get; set; }
    public double? Per36 { get; set; }

    // 3-game rolling averages (null for first two games where window isn't full)
    public double? RollingAvgTsPct { get; set; }
    public double? RollingAvgPer36 { get; set; }
}

// One row in a leaderboard — returned by GET /api/analytics/leaderboards/*
public class LeaderboardEntryDto
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public double Value { get; set; }
}
