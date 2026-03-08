namespace RecLeague.Application.Exceptions;

// Thrown by IngestionService when the same game (same teams + date) already exists.
// IngestionController catches this and returns 409 Conflict.
public class DuplicateGameException : Exception
{
    public DuplicateGameException(DateTime gameDate, string homeTeam, string awayTeam)
        : base($"A game between '{homeTeam}' and '{awayTeam}' on {gameDate:yyyy-MM-dd} already exists.")
    { }
}
