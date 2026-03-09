using RecLeague.Application.DTOs;

namespace RecLeague.Application.Interfaces;

public interface IStatLineService
{
    Task<IEnumerable<StatLineDto>> GetAllAsync();
    Task<StatLineDto?> GetByIdAsync(int id);
    Task<IEnumerable<StatLineDto>> GetByPlayerIdAsync(int playerId);
    Task<IEnumerable<StatLineDto>> GetByGameIdAsync(int gameId);
    Task<StatLineDto> CreateAsync(CreateStatLineDto dto);
    Task UpdateAsync(int id, CreateStatLineDto dto);
    Task DeleteAsync(int id);
}
