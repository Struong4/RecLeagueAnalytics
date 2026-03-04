namespace RecLeague.Domain;

public class Team
{
    // get allows reading the value or the variable, set allows writing the value of the variable
    public int Id { get; set; }
    public string Name {get; set;} = string.Empty;
    public string Season{get; set;} = string.Empty;
    
    // Icollection is a interface for a collection thats like a list, you use this when interacting with
    // EF core because sometimes it can turn your list into a collection so this validate it
    // LINQ methods work on ICollection objects as well
    public ICollection<Player> Players {get;set;} = new List<Player>();
    public ICollection<Game> HomeGames {get;set;} = new List<Game>();
    public ICollection<Game> AwayGames {get;set;} = new List<Game>();
}