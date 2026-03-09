namespace RecLeague.Application.DTOs;

public class PlayerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int JerseyNumber { get; set; }
    public int TeamId { get; set; }
}

public class CreatePlayerDto
{
    public string Name { get; set; } = string.Empty;
    public int JerseyNumber { get; set; }
    public int TeamId { get; set; }
}
