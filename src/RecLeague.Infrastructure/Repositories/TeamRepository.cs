using Microsoft.EntityFrameworkCore;
using RecLeague.Domain;
using RecLeague.Infrastructure.Interfaces;

namespace RecLeague.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    // now this implementation is creating the code for all the functions in interface
    // like C++ prototypes being declared and then the actual code
    private readonly RecLeagueDbContext _db;

    // same example of a dependency injection as controller like DbContext does (adds controller)
    public TeamRepository(RecLeagueDbContext db)
    {
        _db = db;
    }

    // access teams table and then returns the results as a list
    // ToListAsync is the EF Core translation for SQL = SELECT * FROM Teams
    public async Task<IEnumerable<Team>> GetAllAsync()
    {
        return await _db.Teams.ToListAsync();
    }

    // EF Core translate to SELECT * FROM Teams WHERE Id = @id
    public async Task<Team?> GetByIdAsync(int id)
    {
        return await _db.Teams.FindAsync(id);
    }

    // adds new team object into database
    // same as INSERT INTO Teams for SQL, nothing is saved until this is called
    public async Task AddAsync(Team team)
    {
        _db.Teams.Add(team);
        await _db.SaveChangesAsync();
    }

    // updates team
    public async Task UpdateAsync(Team team)
    {
        _db.Teams.Update(team);
        await _db.SaveChangesAsync();
    }

    // marks team for deletion and tells SQL server to delete it
    public async Task DeleteAsync(int id)
    {
        var team = await _db.Teams.FindAsync(id);
        if (team != null)
        {
            _db.Teams.Remove(team);
            await _db.SaveChangesAsync();
        }
    }
}
