using RecLeague.Application.DTOs;
using RecLeague.Application.Interfaces;
using RecLeague.Domain;
using RecLeague.Infrastructure.Interfaces;

namespace RecLeague.Application.Services;

public class StatLineService : IStatLineService
{
    private readonly IStatLineRepository _statLineRepository;

    public StatLineService(IStatLineRepository statLineRepository)
    {
        _statLineRepository = statLineRepository;
    }

    public async Task<IEnumerable<StatLineDto>> GetAllAsync()
    {
        var statLines = await _statLineRepository.GetAllAsync();
        return statLines.Select(s => ToDto(s));
    }

    public async Task<StatLineDto?> GetByIdAsync(int id)
    {
        var statLine = await _statLineRepository.GetByIdAsync(id);
        if (statLine == null) return null;
        return ToDto(statLine);
    }

    public async Task<IEnumerable<StatLineDto>> GetByPlayerIdAsync(int playerId)
    {
        var statLines = await _statLineRepository.GetByPlayerIdAsync(playerId);
        return statLines.Select(s => ToDto(s));
    }

    public async Task<IEnumerable<StatLineDto>> GetByGameIdAsync(int gameId)
    {
        var statLines = await _statLineRepository.GetByGameIdAsync(gameId);
        return statLines.Select(s => ToDto(s));
    }

    public async Task<StatLineDto> CreateAsync(CreateStatLineDto dto)
    {
        var statLine = new StatLine
        {
            PlayerId = dto.PlayerId,
            GameId = dto.GameId,
            Points = dto.Points,
            Rebounds = dto.Rebounds,
            Assists = dto.Assists,
            Steals = dto.Steals,
            Blocks = dto.Blocks,
            Turnovers = dto.Turnovers,
            FGA = dto.FGA,
            FGM = dto.FGM,
            FTA = dto.FTA,
            FTM = dto.FTM,
            MinutesPlayed = dto.MinutesPlayed
        };

        await _statLineRepository.AddAsync(statLine);
        return ToDto(statLine);
    }

    public async Task UpdateAsync(int id, CreateStatLineDto dto)
    {
        var statLine = await _statLineRepository.GetByIdAsync(id);
        if (statLine == null) return;

        statLine.PlayerId = dto.PlayerId;
        statLine.GameId = dto.GameId;
        statLine.Points = dto.Points;
        statLine.Rebounds = dto.Rebounds;
        statLine.Assists = dto.Assists;
        statLine.Steals = dto.Steals;
        statLine.Blocks = dto.Blocks;
        statLine.Turnovers = dto.Turnovers;
        statLine.FGA = dto.FGA;
        statLine.FGM = dto.FGM;
        statLine.FTA = dto.FTA;
        statLine.FTM = dto.FTM;
        statLine.MinutesPlayed = dto.MinutesPlayed;

        await _statLineRepository.UpdateAsync(statLine);
    }

    public async Task DeleteAsync(int id)
    {
        await _statLineRepository.DeleteAsync(id);
    }

    // private helper so we don't repeat the mapping in every method
    private static StatLineDto ToDto(StatLine s) => new StatLineDto
    {
        Id = s.Id,
        PlayerId = s.PlayerId,
        GameId = s.GameId,
        Points = s.Points,
        Rebounds = s.Rebounds,
        Assists = s.Assists,
        Steals = s.Steals,
        Blocks = s.Blocks,
        Turnovers = s.Turnovers,
        FGA = s.FGA,
        FGM = s.FGM,
        FTA = s.FTA,
        FTM = s.FTM,
        MinutesPlayed = s.MinutesPlayed
    };
}
