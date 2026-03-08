using FluentValidation;
using RecLeague.Application.DTOs;

namespace RecLeague.Application.Validators;

public class IngestionRequestValidator : AbstractValidator<IngestionRequestDto>
{
    private static readonly string[] ValidPositions = { "PG", "SG", "SF", "PF", "C" };

    public IngestionRequestValidator()
    {
        RuleFor(x => x.GameDate)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("GameDate cannot be in the future.");

        RuleFor(x => x.Season)
            .InclusiveBetween(1000, 9999).WithMessage("Season must be a 4-digit year.");

        RuleFor(x => x.HomeTeam).NotNull();
        RuleFor(x => x.AwayTeam).NotNull();

        // Home and away teams must have different names
        RuleFor(x => x)
            .Must(x => x.HomeTeam?.Name != x.AwayTeam?.Name)
            .WithName("Teams")
            .WithMessage("Home and away teams must have different names.");

        RuleFor(x => x.HomeTeam).SetValidator(new TeamValidator());
        RuleFor(x => x.AwayTeam).SetValidator(new TeamValidator());
    }
}

public class TeamValidator : AbstractValidator<IngestionTeamDto>
{
    public TeamValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Division).NotEmpty();
        RuleFor(x => x.FinalScore).GreaterThanOrEqualTo(0);

        RuleFor(x => x.Players)
            .Must(p => p.Count >= 5).WithMessage("Each team must have at least 5 players.");

        // Jersey numbers must be unique within the team
        RuleFor(x => x.Players)
            .Must(p => p.Select(pl => pl.JerseyNumber).Distinct().Count() == p.Count)
            .WithMessage("Jersey numbers must be unique within a team.");

        RuleForEach(x => x.Players).SetValidator(new PlayerValidator());
    }
}

public class PlayerValidator : AbstractValidator<IngestionPlayerDto>
{
    private static readonly string[] ValidPositions = { "PG", "SG", "SF", "PF", "C" };

    public PlayerValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();

        RuleFor(x => x.JerseyNumber)
            .InclusiveBetween(0, 99).WithMessage("Jersey number must be between 0 and 99.");

        RuleFor(x => x.Position)
            .Must(p => ValidPositions.Contains(p)).WithMessage("Position must be one of: PG, SG, SF, PF, C.");

        RuleFor(x => x.Stats).NotNull().SetValidator(new StatsValidator());
    }
}

public class StatsValidator : AbstractValidator<IngestionStatsDto>
{
    public StatsValidator()
    {
        // All fields must be non-negative
        RuleFor(x => x.MinutesPlayed).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Points).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Rebounds).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Assists).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Steals).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Blocks).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Turnovers).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PersonalFouls).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FieldGoalsMade).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FieldGoalsAttempted).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ThreePointersMade).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ThreePointersAttempted).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FreeThrowsMade).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FreeThrowsAttempted).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OffensiveRebounds).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DefensiveRebounds).GreaterThanOrEqualTo(0);

        // Made cannot exceed attempted
        RuleFor(x => x).Must(s => s.FieldGoalsMade <= s.FieldGoalsAttempted)
            .WithName("FieldGoals").WithMessage("Field goals made cannot exceed field goals attempted.");

        RuleFor(x => x).Must(s => s.ThreePointersMade <= s.ThreePointersAttempted)
            .WithName("ThreePointers").WithMessage("Three pointers made cannot exceed three pointers attempted.");

        RuleFor(x => x).Must(s => s.FreeThrowsMade <= s.FreeThrowsAttempted)
            .WithName("FreeThrows").WithMessage("Free throws made cannot exceed free throws attempted.");

        // Offensive + defensive rebounds must equal total rebounds
        RuleFor(x => x).Must(s => s.OffensiveRebounds + s.DefensiveRebounds == s.Rebounds)
            .WithName("Rebounds").WithMessage("OffensiveRebounds + DefensiveRebounds must equal Rebounds.");
    }
}
