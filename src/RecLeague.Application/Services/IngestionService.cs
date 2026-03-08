using Microsoft.EntityFrameworkCore;
using RecLeague.Application.DTOs;
using RecLeague.Application.Exceptions;
using RecLeague.Application.Interfaces;
using RecLeague.Domain;
using RecLeague.Infrastructure;

namespace RecLeague.Application.Services;

public class IngestionService : IIngestionService
{
    // IngestionService talks directly to the DbContext so it can wrap everything
    // in a single database transaction. If any step fails, the whole game rolls back.
    private readonly RecLeagueDbContext _db;

    public IngestionService(RecLeagueDbContext db)
    {
        _db = db;
    }

    public async Task<int> IngestGameAsync(IngestionRequestDto dto)
    {
        // BeginTransactionAsync opens a SQL transaction — nothing is committed until
        // CommitAsync is called. If an exception is thrown, RollbackAsync undoes everything.
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var homeTeam = await UpsertTeamAsync(dto.HomeTeam, dto.Season);
            var awayTeam = await UpsertTeamAsync(dto.AwayTeam, dto.Season);

            // Reject the payload if this exact game was already ingested
            var isDuplicate = await _db.Games.AnyAsync(g =>
                g.HomeTeamId == homeTeam.Id &&
                g.AwayTeamId == awayTeam.Id &&
                g.GameDate == dto.GameDate);

            if (isDuplicate)
                throw new DuplicateGameException(dto.GameDate, dto.HomeTeam.Name, dto.AwayTeam.Name);

            var game = new Game
            {
                GameDate = dto.GameDate,
                HomeTeamId = homeTeam.Id,
                AwayTeamId = awayTeam.Id,
                HomeScore = dto.HomeTeam.FinalScore,
                AwayScore = dto.AwayTeam.FinalScore,
                Location = dto.Location,
                Season = dto.Season
            };
            _db.Games.Add(game);
            await _db.SaveChangesAsync(); // gives game.Id a value from the DB

            await IngestTeamPlayersAsync(dto.HomeTeam, homeTeam.Id, game.Id);
            await IngestTeamPlayersAsync(dto.AwayTeam, awayTeam.Id, game.Id);

            await transaction.CommitAsync();
            return game.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw; // re-throw so the controller can handle it
        }
    }

    // Find existing team by Name + Season, or create a new one
    private async Task<Team> UpsertTeamAsync(IngestionTeamDto dto, int season)
    {
        var existing = await _db.Teams.FirstOrDefaultAsync(t =>
            t.Name == dto.Name && t.Season == season.ToString());

        if (existing != null)
            return existing;

        var team = new Team
        {
            Name = dto.Name,
            Division = dto.Division,
            Season = season.ToString()
        };
        _db.Teams.Add(team);
        await _db.SaveChangesAsync();
        return team;
    }

    // For each player in the JSON: find existing by JerseyNumber + TeamId (upsert),
    // then insert their StatLine for this game
    private async Task IngestTeamPlayersAsync(IngestionTeamDto teamDto, int teamId, int gameId)
    {
        foreach (var p in teamDto.Players)
        {
            var player = await _db.Players.FirstOrDefaultAsync(pl =>
                pl.JerseyNumber == p.JerseyNumber && pl.TeamId == teamId);

            if (player == null)
            {
                player = new Player
                {
                    Name = $"{p.FirstName} {p.LastName}",
                    Position = p.Position,
                    JerseyNumber = p.JerseyNumber,
                    TeamId = teamId
                };
                _db.Players.Add(player);
                await _db.SaveChangesAsync(); // needed to get player.Id before creating StatLine
            }

            _db.StatLines.Add(new StatLine
            {
                PlayerId = player.Id,
                GameId = gameId,
                Points = p.Stats.Points,
                Rebounds = p.Stats.Rebounds,
                Assists = p.Stats.Assists,
                Steals = p.Stats.Steals,
                Blocks = p.Stats.Blocks,
                Turnovers = p.Stats.Turnovers,
                PersonalFouls = p.Stats.PersonalFouls,
                FGM = p.Stats.FieldGoalsMade,
                FGA = p.Stats.FieldGoalsAttempted,
                ThreePointersMade = p.Stats.ThreePointersMade,
                ThreePointersAttempted = p.Stats.ThreePointersAttempted,
                FTM = p.Stats.FreeThrowsMade,
                FTA = p.Stats.FreeThrowsAttempted,
                OffensiveRebounds = p.Stats.OffensiveRebounds,
                DefensiveRebounds = p.Stats.DefensiveRebounds,
                MinutesPlayed = p.Stats.MinutesPlayed
            });
        }
        await _db.SaveChangesAsync(); // insert all stat lines for this team at once
    }
}
