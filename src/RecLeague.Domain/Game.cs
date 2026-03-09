namespace RecLeague.Domain;

  public class Game
  {
      public int Id { get; set; }
      public DateTime GameDate { get; set; }
      public int HomeTeamId { get; set; }
      public int AwayTeamId { get; set; }
      public int HomeScore { get; set; }
      public int AwayScore { get; set; }
      public string Location { get; set; } = string.Empty;
      public int Season { get; set; }

      public Team HomeTeam { get; set; } = null!;
      public Team AwayTeam { get; set; } = null!;
      public ICollection<StatLine> StatLines { get; set; } = new List<StatLine>();
  }