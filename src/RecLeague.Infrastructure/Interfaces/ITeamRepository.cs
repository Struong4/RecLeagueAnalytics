using RecLeague.Domain;

namespace RecLeague.Infrastructure.Interfaces;

public interface ITeamRepository
{
    // task means method is async (runs without blocking) with IEnumerable (iterates through collection of teams)
    Task<IEnumerable<Team>> GetAllAsync();
    Task<Team?> GetByIdAsync(int id);
    Task AddAsync(Team team);
    Task UpdateAsync(Team team);
    Task DeleteAsync(int id);
}
