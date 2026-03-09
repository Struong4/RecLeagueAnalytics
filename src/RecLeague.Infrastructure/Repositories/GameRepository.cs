using Microsoft.EntityFrameworkCore;
using RecLeague.Domain;
using RecLeague.Infrastructure.Interfaces;

namespace RecLeague.Infrastructure.Repositories;

public class GameRepository : IGameRepository
{
    private readonly RecLeagueDbContext _db;

    public GameRepository(RecLeagueDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await _db.Games.ToListAsync();
    }

    public async Task<Game?> GetByIdAsync(int id)
    {
        return await _db.Games.FindAsync(id);
    }

    public async Task AddAsync(Game game)
    {
        _db.Games.Add(game);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Game game)
    {
        _db.Games.Update(game);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var game = await _db.Games.FindAsync(id);
        if (game != null)
        {
            _db.Games.Remove(game);
            await _db.SaveChangesAsync();
        }
    }
}
