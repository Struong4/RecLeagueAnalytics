using Microsoft.EntityFrameworkCore;
using RecLeague.Domain;   
namespace RecLeague.Infrastructure;

public class RecLeagueDbContext : DbContext
{
    // constructor is required for DI
    public RecLeagueDbContext(DbContextOptions<RecLeagueDbContext> options) : base(options) { }
    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<Player> Players { get; set; } = null!;
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<StatLine> StatLines { get; set; } = null!; 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasOne(g => g.HomeTeam)
                  .WithMany(t => t.HomeGames)
                  .HasForeignKey(g => g.HomeTeamId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(g => g.AwayTeam)
                  .WithMany(t => t.AwayGames)
                  .HasForeignKey(g => g.AwayTeamId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StatLine>(entity =>
        {
            entity.HasIndex(s => new { s.PlayerId, s.GameId })
                  .IsUnique();
        });
    }
}
