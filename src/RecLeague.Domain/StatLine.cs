namespace RecLeague.Domain;

  public class StatLine
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

      public Player Player { get; set; } = null!;
      public Game Game { get; set; } = null!;
  }