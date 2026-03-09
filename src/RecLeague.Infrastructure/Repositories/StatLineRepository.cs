using Microsoft.EntityFrameworkCore;
using RecLeague.Domain;
using RecLeague.Infrastructure.Interfaces;

namespace RecLeague.Infrastructure.Repositories;

public class StatLineRepository : IStatLineRepository
{
    private readonly RecLeagueDbContext _db;

    public StatLineRepository(RecLeagueDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<StatLine>> GetAllAsync()
    {
        return await _db.StatLines.ToListAsync();
    }

    public async Task<StatLine?> GetByIdAsync(int id)
    {
        return await _db.StatLines.FindAsync(id);
    }

    public async Task<IEnumerable<StatLine>> GetByPlayerIdAsync(int playerId)
    {
        return await _db.StatLines.Where(s => s.PlayerId == playerId).ToListAsync();
    }

    public async Task<IEnumerable<StatLine>> GetByGameIdAsync(int gameId)
    {
        return await _db.StatLines.Where(s => s.GameId == gameId).ToListAsync();
    }

    public async Task AddAsync(StatLine statLine)
    {
        _db.StatLines.Add(statLine);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(StatLine statLine)
    {
        _db.StatLines.Update(statLine);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var statLine = await _db.StatLines.FindAsync(id);
        if (statLine != null)
        {
            _db.StatLines.Remove(statLine);
            await _db.SaveChangesAsync();
        }
    }
}
