using RecLeague.Application.DTOs;
using RecLeague.Application.Interfaces;
using RecLeague.Domain;
using RecLeague.Infrastructure.Interfaces;

namespace RecLeague.Application.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;

    public GameService(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<IEnumerable<GameDto>> GetAllAsync()
    {
        var games = await _gameRepository.GetAllAsync();
        return games.Select(g => new GameDto
        {
            Id = g.Id,
            GameDate = g.GameDate,
            HomeTeamId = g.HomeTeamId,
            AwayTeamId = g.AwayTeamId,
            HomeScore = g.HomeScore,
            AwayScore = g.AwayScore
        });
    }

    public async Task<GameDto?> GetByIdAsync(int id)
    {
        var game = await _gameRepository.GetByIdAsync(id);
        if (game == null) return null;

        return new GameDto
        {
            Id = game.Id,
            GameDate = game.GameDate,
            HomeTeamId = game.HomeTeamId,
            AwayTeamId = game.AwayTeamId,
            HomeScore = game.HomeScore,
            AwayScore = game.AwayScore
        };
    }

    public async Task<GameDto> CreateAsync(CreateGameDto dto)
    {
        var game = new Game
        {
            GameDate = dto.GameDate,
            HomeTeamId = dto.HomeTeamId,
            AwayTeamId = dto.AwayTeamId,
            HomeScore = dto.HomeScore,
            AwayScore = dto.AwayScore
        };

        await _gameRepository.AddAsync(game);

        return new GameDto
        {
            Id = game.Id,
            GameDate = game.GameDate,
            HomeTeamId = game.HomeTeamId,
            AwayTeamId = game.AwayTeamId,
            HomeScore = game.HomeScore,
            AwayScore = game.AwayScore
        };
    }

    public async Task UpdateAsync(int id, CreateGameDto dto)
    {
        var game = await _gameRepository.GetByIdAsync(id);
        if (game == null) return;

        game.GameDate = dto.GameDate;
        game.HomeTeamId = dto.HomeTeamId;
        game.AwayTeamId = dto.AwayTeamId;
        game.HomeScore = dto.HomeScore;
        game.AwayScore = dto.AwayScore;

        await _gameRepository.UpdateAsync(game);
    }

    public async Task DeleteAsync(int id)
    {
        await _gameRepository.DeleteAsync(id);
    }
}
