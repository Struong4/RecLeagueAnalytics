namespace RecLeague.Application.DTOs;

// Top-level object that maps the entire incoming JSON body
public class IngestionRequestDto
{
    public DateTime GameDate { get; set; }
    public int Season { get; set; }
    public string Location { get; set; } = string.Empty;
    public IngestionTeamDto HomeTeam { get; set; } = null!;
    public IngestionTeamDto AwayTeam { get; set; } = null!;
}

// One team's data inside the payload
public class IngestionTeamDto
{
    public string Name { get; set; } = string.Empty;
    public string Division { get; set; } = string.Empty;
    public int FinalScore { get; set; }
    public List<IngestionPlayerDto> Players { get; set; } = new();
}

// One player's data inside a team
public class IngestionPlayerDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int JerseyNumber { get; set; }
    public string Position { get; set; } = string.Empty;
    public IngestionStatsDto Stats { get; set; } = null!;
}

// All 16 stat fields for a single player in a single game
public class IngestionStatsDto
{
    public int MinutesPlayed { get; set; }
    public int Points { get; set; }
    public int Rebounds { get; set; }
    public int Assists { get; set; }
    public int Steals { get; set; }
    public int Blocks { get; set; }
    public int Turnovers { get; set; }
    public int PersonalFouls { get; set; }
    public int FieldGoalsMade { get; set; }
    public int FieldGoalsAttempted { get; set; }
    public int ThreePointersMade { get; set; }
    public int ThreePointersAttempted { get; set; }
    public int FreeThrowsMade { get; set; }
    public int FreeThrowsAttempted { get; set; }
    public int OffensiveRebounds { get; set; }
    public int DefensiveRebounds { get; set; }
}
