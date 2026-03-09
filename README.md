# RecLeague Analytics

A recreational league basketball management and analytics platform. Built with ASP.NET Core 8, SQL Server, and Clean Architecture — designed to ingest game data via JSON and surface basketball metrics through a RESTful API.

---

## Features

- **Full CRUD API** for teams, players, games, and stat lines
- **JSON Ingestion Pipeline** — POST a single game payload to persist teams, players, and stats in one atomic transaction
- **Upsert Logic** — re-ingesting a roster won't duplicate players or teams
- **Duplicate Detection** — re-submitting the same game returns `409 Conflict`
- **FluentValidation** — payload rejected at the controller before any DB work if rules are violated
- **Basketball Analytics** — True Shooting %, PER/36, Usage Rate, AST/TO ratio, leaderboards, and per-game trend data with rolling averages
- **Swagger UI** — interactive API docs available out of the box
- **Docker Compose** — entire stack starts with one command, no manual setup required
- **Jenkins CI/CD** — automated build, test, and Docker image pipeline

---

## Tech Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 (SQL Server) |
| Validation | FluentValidation 12 |
| API Docs | Swashbuckle / Swagger UI |
| Database | SQL Server 2022 (Docker) |
| Testing | xUnit + Moq + FluentAssertions |
| Containerization | Docker + Docker Compose |
| CI/CD | Jenkins |

---

## Architecture

Clean layered architecture — each layer only references the one directly below it.

```
RecLeague.API  →  RecLeague.Application  →  RecLeague.Infrastructure  →  RecLeague.Domain
(controllers)      (services, DTOs,           (EF Core, repositories)      (entities only,
                    validators, analytics)                                   no dependencies)
```

**Request flow:** `HTTP Request → Controller → Service → Repository → DbContext → SQL Server`

---

## Database Schema

```
Team ──< Player ──< StatLine
Team ──< Game   ──< StatLine
```

- `StatLine` is the central table — every analytic traces back to it
- Unique index on `(PlayerId, GameId)` prevents duplicate stat entries at the database level
- `Game` uses `DeleteBehavior.Restrict` on both team FK relationships to protect game history

---

## Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Run the full stack

```bash
docker-compose up --build
```

Swagger UI: `http://localhost:8080/swagger`

That's it. Docker Compose handles SQL Server, runs migrations automatically, and starts the API.

### Local development (without Docker)

```bash
# Start SQL Server container
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=RecLeague@2025!" \
  -p 1433:1433 --name recleague-sql -d \
  mcr.microsoft.com/mssql/server:2022-latest

# Run the API
dotnet run --project src/RecLeague.API/RecLeague.API.csproj
```

Swagger UI: `http://localhost:5000/swagger`

---

## API Endpoints

### Teams
| Method | Route | Description |
|---|---|---|
| GET | `/api/teams` | Get all teams |
| GET | `/api/teams/{id}` | Get team by ID |
| POST | `/api/teams` | Create a team |
| PUT | `/api/teams/{id}` | Update a team |
| DELETE | `/api/teams/{id}` | Delete a team |

### Players
| Method | Route | Description |
|---|---|---|
| GET | `/api/players` | Get all players |
| GET | `/api/players/{id}` | Get player by ID |
| POST | `/api/players` | Create a player |
| PUT | `/api/players/{id}` | Update a player |
| DELETE | `/api/players/{id}` | Delete a player |

### Games
| Method | Route | Description |
|---|---|---|
| GET | `/api/games` | Get all games |
| GET | `/api/games/{id}` | Get game by ID |
| POST | `/api/games` | Create a game |
| PUT | `/api/games/{id}` | Update a game |
| DELETE | `/api/games/{id}` | Delete a game |

### Stats
| Method | Route | Description |
|---|---|---|
| GET | `/api/stats` | Get all stat lines |
| GET | `/api/stats/{id}` | Get stat line by ID |
| POST | `/api/stats` | Create a stat line |
| PUT | `/api/stats/{id}` | Update a stat line |
| DELETE | `/api/stats/{id}` | Delete a stat line |

### Ingestion
| Method | Route | Description |
|---|---|---|
| POST | `/api/ingestion/game` | Ingest a full game JSON payload |

### Analytics
| Method | Route | Description |
|---|---|---|
| GET | `/api/analytics/player/{id}` | Player efficiency metrics and averages |
| GET | `/api/analytics/player/{id}/trends` | Per-game trend data with rolling averages |
| GET | `/api/analytics/team/{id}` | Team win%, PPG, efficiency stats |
| GET | `/api/analytics/leaderboards/scoring` | Top 10 players by points per game |
| GET | `/api/analytics/leaderboards/efficiency` | Top 10 players by True Shooting % |
| GET | `/api/analytics/season/{year}/summary` | Season totals and top performers |

---

## JSON Ingestion

POST a single JSON payload to `/api/ingestion/game` to persist an entire game — teams, players, and all stat lines — in one atomic database transaction.

```json
{
  "gameDate": "2025-03-15T19:30:00Z",
  "season": 2025,
  "location": "Westside Community Center",
  "homeTeam": {
    "name": "Westside Ballers",
    "division": "Division A",
    "finalScore": 82,
    "players": [
      {
        "firstName": "Marcus",
        "lastName": "Thompson",
        "jerseyNumber": 23,
        "position": "PG",
        "stats": {
          "minutesPlayed": 32,
          "points": 18,
          "rebounds": 4,
          "assists": 9,
          "steals": 2,
          "blocks": 0,
          "turnovers": 3,
          "personalFouls": 2,
          "fieldGoalsMade": 7,
          "fieldGoalsAttempted": 14,
          "threePointersMade": 2,
          "threePointersAttempted": 5,
          "freeThrowsMade": 2,
          "freeThrowsAttempted": 2,
          "offensiveRebounds": 1,
          "defensiveRebounds": 3
        }
      }
    ]
  },
  "awayTeam": { }
}
```

**Validation rules:**
- `gameDate` must not be in the future
- `season` must be a 4-digit year
- Home and away team names must differ
- Each team must have at least 5 players
- Jersey numbers must be 0–99 and unique within a team
- Position must be one of: `PG`, `SG`, `SF`, `PF`, `C`
- All stat fields must be ≥ 0
- Made cannot exceed attempted (FG, 3P, FT)
- `offensiveRebounds + defensiveRebounds` must equal `rebounds`

---

## Analytics Metrics

| Metric | Formula |
|---|---|
| True Shooting % | `PTS / (2 * (FGA + 0.44 * FTA))` |
| FG% | `FGM / FGA` |
| 3P% | `3PM / 3PA` |
| FT% | `FTM / FTA` |
| AST/TO Ratio | `AST / TOV` |
| PER/36 | `(PTS + REB + AST + STL + BLK - missed FG - missed FT - TOV) / MIN * 36` |
| Usage Rate | `(FGA + 0.44*FTA + TOV) / (Team FGA + 0.44*Team FTA + Team TOV)` |

---

## Running Tests

```bash
dotnet test RecLeagueAnalytics.sln
```

---

## CI/CD

Jenkins runs locally at `http://localhost:8090`. The pipeline stages are:

1. **Checkout** — pulls latest code from GitHub
2. **Restore** — restores NuGet packages
3. **Build** — compiles in Release mode
4. **Test** — runs all unit tests
5. **Docker Build** — builds the API Docker image

