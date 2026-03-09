using RecLeague.Application.DTOs;

namespace RecLeague.Application.Interfaces;

public interface IGameService
{
    Task<IEnumerable<GameDto>> GetAllAsync();
    Task<GameDto?> GetByIdAsync(int id);
    Task<GameDto> CreateAsync(CreateGameDto dto);
    Task UpdateAsync(int id, CreateGameDto dto);
    Task DeleteAsync(int id);
}
