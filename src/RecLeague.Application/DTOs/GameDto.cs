namespace RecLeague.Application.DTOs;

public class GameDto
{
    public int Id { get; set; }
    public DateTime GameDate { get; set; }
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Season { get; set; }
}

public class CreateGameDto
{
    public DateTime GameDate { get; set; }
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Season { get; set; }
}
