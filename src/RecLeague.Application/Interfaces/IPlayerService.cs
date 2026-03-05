using RecLeague.Application.DTOs;

namespace RecLeague.Application.Interfaces;

public interface IPlayerService
{
    Task<IEnumerable<PlayerDto>> GetAllAsync();
    Task<PlayerDto?> GetByIdAsync(int id);
    Task<IEnumerable<PlayerDto>> GetByTeamIdAsync(int teamId);
    Task<PlayerDto> CreateAsync(CreatePlayerDto dto);
    Task UpdateAsync(int id, CreatePlayerDto dto);
    Task DeleteAsync(int id);
}
