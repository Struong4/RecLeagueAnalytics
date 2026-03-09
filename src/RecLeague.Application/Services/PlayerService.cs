using RecLeague.Application.DTOs;
using RecLeague.Application.Interfaces;
using RecLeague.Domain;
using RecLeague.Infrastructure.Interfaces;

namespace RecLeague.Application.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;

    public PlayerService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<IEnumerable<PlayerDto>> GetAllAsync()
    {
        var players = await _playerRepository.GetAllAsync();
        return players.Select(p => new PlayerDto
        {
            Id = p.Id,
            Name = p.Name,
            JerseyNumber = p.JerseyNumber,
            TeamId = p.TeamId
        });
    }

    public async Task<PlayerDto?> GetByIdAsync(int id)
    {
        var player = await _playerRepository.GetByIdAsync(id);
        if (player == null) return null;

        return new PlayerDto
        {
            Id = player.Id,
            Name = player.Name,
            JerseyNumber = player.JerseyNumber,
            TeamId = player.TeamId
        };
    }

    public async Task<IEnumerable<PlayerDto>> GetByTeamIdAsync(int teamId)
    {
        var players = await _playerRepository.GetByTeamIdAsync(teamId);
        return players.Select(p => new PlayerDto
        {
            Id = p.Id,
            Name = p.Name,
            JerseyNumber = p.JerseyNumber,
            TeamId = p.TeamId
        });
    }

    public async Task<PlayerDto> CreateAsync(CreatePlayerDto dto)
    {
        var player = new Player
        {
            Name = dto.Name,
            JerseyNumber = dto.JerseyNumber,
            TeamId = dto.TeamId
        };

        await _playerRepository.AddAsync(player);

        return new PlayerDto
        {
            Id = player.Id,
            Name = player.Name,
            JerseyNumber = player.JerseyNumber,
            TeamId = player.TeamId
        };
    }

    public async Task UpdateAsync(int id, CreatePlayerDto dto)
    {
        var player = await _playerRepository.GetByIdAsync(id);
        if (player == null) return;

        player.Name = dto.Name;
        player.JerseyNumber = dto.JerseyNumber;
        player.TeamId = dto.TeamId;

        await _playerRepository.UpdateAsync(player);
    }

    public async Task DeleteAsync(int id)
    {
        await _playerRepository.DeleteAsync(id);
    }
}
