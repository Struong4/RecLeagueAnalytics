using Microsoft.EntityFrameworkCore;
using RecLeague.Domain;
using RecLeague.Infrastructure.Interfaces;

namespace RecLeague.Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly RecLeagueDbContext _db;

    public PlayerRepository(RecLeagueDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Player>> GetAllAsync()
    {
        return await _db.Players.ToListAsync();
    }

    public async Task<Player?> GetByIdAsync(int id)
    {
        return await _db.Players.FindAsync(id);
    }

    public async Task<IEnumerable<Player>> GetByTeamIdAsync(int teamId)
    {
        return await _db.Players.Where(p => p.TeamId == teamId).ToListAsync();
    }

    public async Task AddAsync(Player player)
    {
        _db.Players.Add(player);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Player player)
    {
        _db.Players.Update(player);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var player = await _db.Players.FindAsync(id);
        if (player != null)
        {
            _db.Players.Remove(player);
            await _db.SaveChangesAsync();
        }
    }
}
