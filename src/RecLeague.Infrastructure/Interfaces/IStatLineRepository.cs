using RecLeague.Domain;

namespace RecLeague.Infrastructure.Interfaces;

public interface IStatLineRepository
{
    Task<IEnumerable<StatLine>> GetAllAsync();
    Task<StatLine?> GetByIdAsync(int id);
    Task<IEnumerable<StatLine>> GetByPlayerIdAsync(int playerId);
    Task<IEnumerable<StatLine>> GetByGameIdAsync(int gameId);
    Task AddAsync(StatLine statLine);
    Task UpdateAsync(StatLine statLine);
    Task DeleteAsync(int id);
}
