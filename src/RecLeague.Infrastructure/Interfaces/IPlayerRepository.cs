using RecLeague.Domain;

namespace RecLeague.Infrastructure.Interfaces;

public interface IPlayerRepository
{
    Task<IEnumerable<Player>> GetAllAsync();
    Task<Player?> GetByIdAsync(int id);
    Task<IEnumerable<Player>> GetByTeamIdAsync(int teamId);
    Task AddAsync(Player player);
    Task UpdateAsync(Player player);
    Task DeleteAsync(int id);
}
