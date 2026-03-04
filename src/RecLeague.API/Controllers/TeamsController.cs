using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecLeague.Infrastructure;

namespace RecLeague.API.Controllers;

// tells ASP.NET this class handles HHTP requests
[ApiController]
// fixing the route like pointing to a directory
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly RecLeagueDbContext _db;

    // dependency injection that basically adds controller and gives it a service
    // automatcally passes the dbcontext registerd in Program.cs
    public TeamsController(RecLeagueDbContext db)
    {
        _db = db;
    }

    // wraps the list of teams in a HHTP response with the data as JSON
    // makes it async so it doesnt block the thread (like how bedrock automatically blocks)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var teams = await _db.Teams.ToListAsync();
        return Ok(teams);
    }
}
