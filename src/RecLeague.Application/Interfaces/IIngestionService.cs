using RecLeague.Application.DTOs;

namespace RecLeague.Application.Interfaces;

public interface IIngestionService
{
    // Ingests a full game JSON payload. Returns the new Game Id.
    // Throws DuplicateGameException if the same game already exists.
    Task<int> IngestGameAsync(IngestionRequestDto dto);
}
