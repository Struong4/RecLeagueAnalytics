using RecLeague.Application.DTOs;

namespace RecLeague.Application.Interfaces;

public interface ITeamService
{
    // takes care of CRUD operations transitioning C# into SQL language
    Task<IEnumerable<TeamDto>> GetAllAsync();
    Task<TeamDto?> GetByIdAsync(int id);
    Task<TeamDto> CreateAsync(CreateTeamDto dto);
    Task UpdateAsync(int id, CreateTeamDto dto);
    Task DeleteAsync(int id);
}
