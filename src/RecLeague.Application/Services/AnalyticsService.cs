using Microsoft.EntityFrameworkCore;
using RecLeague.Application.DTOs;
using RecLeague.Application.Interfaces;
using RecLeague.Infrastructure;

namespace RecLeague.Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly RecLeagueDbContext _db;

    public AnalyticsService(RecLeagueDbContext db)
    {
        _db = db;
    }

    // -------------------------------------------------------------------------
    // Player Analytics
    // -------------------------------------------------------------------------

    public async Task<PlayerAnalyticsDto?> GetPlayerAnalyticsAsync(int playerId)
    {
        var player = await _db.Players
            .Include(p => p.Team)
            .Include(p => p.StatLines)
            .FirstOrDefaultAsync(p => p.Id == playerId);

        if (player == null) return null;

        var stats = player.StatLines.ToList();
        if (!stats.Any())
            return new PlayerAnalyticsDto { PlayerId = playerId, PlayerName = player.Name, TeamName = player.Team.Name };

        int games   = stats.Count;
        int totPts  = stats.Sum(s => s.Points);
        int totReb  = stats.Sum(s => s.Rebounds);
        int totAst  = stats.Sum(s => s.Assists);
        int totStl  = stats.Sum(s => s.Steals);
        int totBlk  = stats.Sum(s => s.Blocks);
        int totTov  = stats.Sum(s => s.Turnovers);
        int totFga  = stats.Sum(s => s.FGA);
        int totFgm  = stats.Sum(s => s.FGM);
        int totFta  = stats.Sum(s => s.FTA);
        int totFtm  = stats.Sum(s => s.FTM);
        int tot3pa  = stats.Sum(s => s.ThreePointersAttempted);
        int tot3pm  = stats.Sum(s => s.ThreePointersMade);
        int totMin  = stats.Sum(s => s.MinutesPlayed);

        // Usage rate needs teammates' totals across the same games
        var gameIds = stats.Select(s => s.GameId).ToList();
        var teamStats = await _db.StatLines
            .Where(s => gameIds.Contains(s.GameId) &&
                        _db.Players.Any(p => p.Id == s.PlayerId && p.TeamId == player.TeamId))
            .ToListAsync();

        var teamFga = teamStats.Sum(s => s.FGA);
        var teamFta = teamStats.Sum(s => s.FTA);
        var teamTov = teamStats.Sum(s => s.Turnovers);
        var teamPossessions = teamFga + 0.44 * teamFta + teamTov;

        return new PlayerAnalyticsDto
        {
            PlayerId       = playerId,
            PlayerName     = player.Name,
            TeamName       = player.Team.Name,
            GamesPlayed    = games,
            PointsPerGame   = Round((double)totPts / games),
            ReboundsPerGame = Round((double)totReb / games),
            AssistsPerGame  = Round((double)totAst / games),
            StealsPerGame   = Round((double)totStl / games),
            BlocksPerGame   = Round((double)totBlk / games),
            TrueShootingPct = CalcTsPct(totPts, totFga, totFta),
            FieldGoalPct    = totFga == 0 ? null : Round((double)totFgm / totFga),
            ThreePointPct   = tot3pa == 0 ? null : Round((double)tot3pm / tot3pa),
            FreeThrowPct    = totFta == 0 ? null : Round((double)totFtm / totFta),
            AstToRatio      = totTov == 0 ? null : Round((double)totAst / totTov),
            Per36           = CalcPer36(totPts, totReb, totAst, totStl, totBlk, totFga, totFgm, totFta, totFtm, totTov, totMin),
            UsageRate       = teamPossessions == 0 ? null : Round((totFga + 0.44 * totFta + totTov) / teamPossessions)
        };
    }

    public async Task<List<PlayerTrendDto>> GetPlayerTrendsAsync(int playerId)
    {
        var statLines = await _db.StatLines
            .Include(s => s.Game)
            .Where(s => s.PlayerId == playerId)
            .OrderBy(s => s.Game.GameDate)
            .ToListAsync();

        var trends = statLines.Select(s => new PlayerTrendDto
        {
            GameId          = s.GameId,
            GameDate        = s.Game.GameDate,
            Points          = s.Points,
            TrueShootingPct = CalcTsPct(s.Points, s.FGA, s.FTA),
            Per36           = CalcPer36(s.Points, s.Rebounds, s.Assists, s.Steals, s.Blocks,
                                        s.FGA, s.FGM, s.FTA, s.FTM, s.Turnovers, s.MinutesPlayed)
        }).ToList();

        // 3-game rolling averages — window slides forward game by game
        for (int i = 0; i < trends.Count; i++)
        {
            var window   = trends.Skip(Math.Max(0, i - 2)).Take(Math.Min(3, i + 1)).ToList();
            var tsValues  = window.Where(t => t.TrueShootingPct.HasValue).Select(t => t.TrueShootingPct!.Value).ToList();
            var perValues = window.Where(t => t.Per36.HasValue).Select(t => t.Per36!.Value).ToList();
            trends[i].RollingAvgTsPct  = tsValues.Any()  ? Round(tsValues.Average())  : null;
            trends[i].RollingAvgPer36  = perValues.Any() ? Round(perValues.Average()) : null;
        }

        return trends;
    }

    // -------------------------------------------------------------------------
    // Team Analytics
    // -------------------------------------------------------------------------

    public async Task<TeamAnalyticsDto?> GetTeamAnalyticsAsync(int teamId)
    {
        var team = await _db.Teams.FindAsync(teamId);
        if (team == null) return null;

        // All games this team played in (home or away)
        var games = await _db.Games
            .Where(g => g.HomeTeamId == teamId || g.AwayTeamId == teamId)
            .ToListAsync();

        if (!games.Any())
            return new TeamAnalyticsDto { TeamId = teamId, TeamName = team.Name, Season = team.Season };

        int played = games.Count;
        int wins   = games.Count(g =>
            (g.HomeTeamId == teamId && g.HomeScore > g.AwayScore) ||
            (g.AwayTeamId == teamId && g.AwayScore > g.HomeScore));
        int losses = played - wins;

        // Points scored and allowed per game (from the Game score fields)
        double ptsScored  = games.Average(g => g.HomeTeamId == teamId ? g.HomeScore : g.AwayScore);
        double ptsAllowed = games.Average(g => g.HomeTeamId == teamId ? g.AwayScore : g.HomeScore);

        // Rebounds and assists from all team stat lines
        var gameIds   = games.Select(g => g.Id).ToList();
        var teamStats = await _db.StatLines
            .Where(s => gameIds.Contains(s.GameId) &&
                        _db.Players.Any(p => p.Id == s.PlayerId && p.TeamId == teamId))
            .ToListAsync();

        double rpg = played == 0 ? 0 : Round(teamStats.Sum(s => s.Rebounds) / (double)played);
        double apg = played == 0 ? 0 : Round(teamStats.Sum(s => s.Assists)  / (double)played);

        return new TeamAnalyticsDto
        {
            TeamId               = teamId,
            TeamName             = team.Name,
            Season               = team.Season,
            GamesPlayed          = played,
            Wins                 = wins,
            Losses               = losses,
            WinPct               = Round((double)wins / played),
            PointsPerGame        = Round(ptsScored),
            ReboundsPerGame      = rpg,
            AssistsPerGame       = apg,
            OffensiveEfficiency  = Round(ptsScored),
            DefensiveEfficiency  = Round(ptsAllowed),
            NetEfficiency        = Round(ptsScored - ptsAllowed)
        };
    }

    // -------------------------------------------------------------------------
    // Leaderboards
    // -------------------------------------------------------------------------

    public async Task<List<LeaderboardEntryDto>> GetScoringLeaderboardAsync()
    {
        var players = await _db.Players
            .Include(p => p.Team)
            .Include(p => p.StatLines)
            .ToListAsync();

        return players
            .Where(p => p.StatLines.Count >= 1)
            .Select(p => new LeaderboardEntryDto
            {
                PlayerId   = p.Id,
                PlayerName = p.Name,
                TeamName   = p.Team.Name,
                Value      = Round(p.StatLines.Average(s => (double)s.Points))
            })
            .OrderByDescending(e => e.Value)
            .Take(10)
            .ToList();
    }

    public async Task<List<LeaderboardEntryDto>> GetEfficiencyLeaderboardAsync()
    {
        var players = await _db.Players
            .Include(p => p.Team)
            .Include(p => p.StatLines)
            .ToListAsync();

        return players
            .Where(p => p.StatLines.Count >= 1)
            .Select(p =>
            {
                int pts = p.StatLines.Sum(s => s.Points);
                int fga = p.StatLines.Sum(s => s.FGA);
                int fta = p.StatLines.Sum(s => s.FTA);
                return new LeaderboardEntryDto
                {
                    PlayerId   = p.Id,
                    PlayerName = p.Name,
                    TeamName   = p.Team.Name,
                    Value      = CalcTsPct(pts, fga, fta) ?? 0
                };
            })
            .Where(e => e.Value > 0)
            .OrderByDescending(e => e.Value)
            .Take(10)
            .ToList();
    }

    // -------------------------------------------------------------------------
    // Season Summary
    // -------------------------------------------------------------------------

    public async Task<SeasonSummaryDto?> GetSeasonSummaryAsync(int year)
    {
        var games = await _db.Games.Where(g => g.Season == year).ToListAsync();
        if (!games.Any()) return null;

        var gameIds    = games.Select(g => g.Id).ToList();
        var teamIds    = games.SelectMany(g => new[] { g.HomeTeamId, g.AwayTeamId }).Distinct().ToList();
        var playerIds  = await _db.StatLines
            .Where(s => gameIds.Contains(s.GameId))
            .Select(s => s.PlayerId)
            .Distinct()
            .CountAsync();

        // Top scorer in this season
        var seasonStats = await _db.StatLines
            .Include(s => s.Player).ThenInclude(p => p.Team)
            .Where(s => gameIds.Contains(s.GameId))
            .ToListAsync();

        var topScorer = seasonStats
            .GroupBy(s => s.PlayerId)
            .Select(g => new LeaderboardEntryDto
            {
                PlayerId   = g.Key,
                PlayerName = g.First().Player.Name,
                TeamName   = g.First().Player.Team.Name,
                Value      = Round(g.Average(s => (double)s.Points))
            })
            .OrderByDescending(e => e.Value)
            .FirstOrDefault();

        // Top efficiency in this season
        var topEfficiency = seasonStats
            .GroupBy(s => s.PlayerId)
            .Select(g =>
            {
                int pts = g.Sum(s => s.Points);
                int fga = g.Sum(s => s.FGA);
                int fta = g.Sum(s => s.FTA);
                return new LeaderboardEntryDto
                {
                    PlayerId   = g.Key,
                    PlayerName = g.First().Player.Name,
                    TeamName   = g.First().Player.Team.Name,
                    Value      = CalcTsPct(pts, fga, fta) ?? 0
                };
            })
            .Where(e => e.Value > 0)
            .OrderByDescending(e => e.Value)
            .FirstOrDefault();

        return new SeasonSummaryDto
        {
            Season        = year,
            TotalGames    = games.Count,
            TotalTeams    = teamIds.Count,
            TotalPlayers  = playerIds,
            TopScorer     = topScorer,
            TopEfficiency = topEfficiency
        };
    }

    // -------------------------------------------------------------------------
    // Formula helpers — private static so they're easy to unit test
    // -------------------------------------------------------------------------

    // True Shooting %: PTS / (2 * (FGA + 0.44 * FTA))
    private static double? CalcTsPct(int pts, int fga, int fta)
    {
        var denominator = 2.0 * (fga + 0.44 * fta);
        return denominator == 0 ? null : Round(pts / denominator);
    }

    // Simplified PER per 36 minutes:
    // (PTS + REB + AST + STL + BLK - missed FG - missed FT - TOV) / MIN * 36
    private static double? CalcPer36(int pts, int reb, int ast, int stl, int blk,
                                      int fga, int fgm, int fta, int ftm, int tov, int min)
    {
        if (min == 0) return null;
        var raw = pts + reb + ast + stl + blk - (fga - fgm) - (fta - ftm) - tov;
        return Round((double)raw / min * 36);
    }

    private static double Round(double value) => Math.Round(value, 3);
}
