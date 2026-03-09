using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RecLeague.Application.Interfaces;
using RecLeague.Application.Services;
using RecLeague.Application.Validators;
using RecLeague.Infrastructure;
using RecLeague.Infrastructure.Interfaces;
using RecLeague.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<RecLeagueDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IStatLineRepository, StatLineRepository>();

// Services
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IStatLineService, StatLineService>();
builder.Services.AddScoped<IIngestionService, IngestionService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

// FluentValidation — scans Application assembly and registers all validators
builder.Services.AddValidatorsFromAssemblyContaining<IngestionRequestValidator>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Auto-apply migrations on startup with retry — SQL Server takes a few seconds to be
// ready inside Docker even after the container starts, so we retry until it responds.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RecLeagueDbContext>();
    var retries = 10;
    while (retries-- > 0)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch
        {
            if (retries == 0) throw;
            Thread.Sleep(3000); // wait 3 seconds before retrying
        }
    }
}

// Swagger available in all environments so it works inside Docker
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
