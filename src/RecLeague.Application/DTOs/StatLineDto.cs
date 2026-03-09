namespace RecLeague.Application.DTOs;

public class StatLineDto
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int GameId { get; set; }
    public int Points { get; set; }
    public int Rebounds { get; set; }
    public int Assists { get; set; }
    public int Steals { get; set; }
    public int Blocks { get; set; }
    public int Turnovers { get; set; }
    public int FGA { get; set; }
    public int FGM { get; set; }
    public int FTA { get; set; }
    public int FTM { get; set; }
    public int MinutesPlayed { get; set; }
}

public class CreateStatLineDto
{
    public int PlayerId { get; set; }
    public int GameId { get; set; }
    public int Points { get; set; }
    public int Rebounds { get; set; }
    public int Assists { get; set; }
    public int Steals { get; set; }
    public int Blocks { get; set; }
    public int Turnovers { get; set; }
    public int FGA { get; set; }
    public int FGM { get; set; }
    public int FTA { get; set; }
    public int FTM { get; set; }
    public int MinutesPlayed { get; set; }
}
