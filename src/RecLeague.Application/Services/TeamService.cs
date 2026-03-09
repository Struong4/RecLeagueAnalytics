using RecLeague.Application.DTOs;
using RecLeague.Application.Interfaces;
using RecLeague.Domain;
using RecLeague.Infrastructure.Interfaces;

namespace RecLeague.Application.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;

    //dependency injection of controller
    public TeamService(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    // calls the repo to get domain entities from database, 
    // uses LINQ queries to loop through every team and turn 
    // them into a list of DTO (data transfer objects) 
    public async Task<IEnumerable<TeamDto>> GetAllAsync()
    {
        var teams = await _teamRepository.GetAllAsync();
        return teams.Select(t => new TeamDto
        {
            Id = t.Id,
            Name = t.Name,
            Season = t.Season
        });
    }
    
    public async Task<TeamDto?> GetByIdAsync(int id)
    {
        var team = await _teamRepository.GetByIdAsync(id);
        if (team == null) return null;

        return new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            Season = team.Season
        };
    }

    // takes CreateTeamDto from the controller, maps it into a 
    // team domain entity so EF Core can understand it.  
    // saves it via repository and then EF Core fills in the ID
    // and then puts it back into a TeamDTO and return it so the caller
    // gets back the created team with the new id (DTO focus on transfering through layers)
    public async Task<TeamDto> CreateAsync(CreateTeamDto dto)
    {
        var team = new Team
        {
            Name = dto.Name,
            Season = dto.Season
        };

        await _teamRepository.AddAsync(team);

        return new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            Season = team.Season
        };
    }

    public async Task UpdateAsync(int id, CreateTeamDto dto)
    {
        var team = await _teamRepository.GetByIdAsync(id);
        if (team == null) return;

        team.Name = dto.Name;
        team.Season = dto.Season;

        await _teamRepository.UpdateAsync(team);
    }

    public async Task DeleteAsync(int id)
    {
        await _teamRepository.DeleteAsync(id);
    }
}
