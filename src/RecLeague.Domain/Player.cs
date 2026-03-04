namespace RecLeague.Domain;

public class Player
{
    public int Id {get;set;}
    public string Name {get;set;} = string.Empty;
    public int JerseyNumber {get;set;}
    public int TeamId {get;set;}
    public Team Team {get;set;} = null!;
    public ICollection<StatLine> StatLines {get;set;} = new List<StatLine>();
}