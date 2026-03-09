using FluentAssertions;
using Moq;
using RecLeague.Application.DTOs;
using RecLeague.Application.Services;
using RecLeague.Domain;
using RecLeague.Infrastructure.Interfaces;

namespace RecLeague.Application.Tests.Services;

public class TeamServiceTests
{
    // fake repository - no real database needed
    private readonly Mock<ITeamRepository> _mockRepo;
    private readonly TeamService _teamService;

    public TeamServiceTests()
    {
        _mockRepo = new Mock<ITeamRepository>();
        _teamService = new TeamService(_mockRepo.Object);
    }

    // checks that GetAll returns every team from the repo as a DTO
    [Fact]
    public async Task GetAllAsync_ReturnsAllTeamsAsDtos()
    {
        // Arrange - set up fake data the repo will return
        var fakeTeams = new List<Team>
        {
            new Team { Id = 1, Name = "Westside Ballers", Season = "2025" },
            new Team { Id = 2, Name = "Eastside Crew", Season = "2025" }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(fakeTeams);

        // Act - call the service method
        var result = await _teamService.GetAllAsync();

        // Assert - check the result is correct
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Westside Ballers");
    }

    // checks that GetById returns the correct team when it exists
    [Fact]
    public async Task GetByIdAsync_WhenTeamExists_ReturnsTeamDto()
    {
        // Arrange
        var fakeTeam = new Team { Id = 1, Name = "Westside Ballers", Season = "2025" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeTeam);

        // Act
        var result = await _teamService.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Westside Ballers");
    }

    // checks that GetById returns null when no team with that id exists
    [Fact]
    public async Task GetByIdAsync_WhenTeamDoesNotExist_ReturnsNull()
    {
        // Arrange - repo returns null for id 99
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Team?)null);

        // Act
        var result = await _teamService.GetByIdAsync(99);

        // Assert
        result.Should().BeNull();
    }

    // checks that Create saves the team to the repo and returns it as a DTO
    [Fact]
    public async Task CreateAsync_AddsTeamAndReturnsDto()
    {
        // Arrange
        var dto = new CreateTeamDto { Name = "New Team", Season = "2025" };
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Team>())).Returns(Task.CompletedTask);

        // Act
        var result = await _teamService.CreateAsync(dto);

        // Assert - verify the repo was called once and result has correct data
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Team>()), Times.Once);
        result.Name.Should().Be("New Team");
        result.Season.Should().Be("2025");
    }

    // checks that Update fetches the team, applies the new values, and saves it
    [Fact]
    public async Task UpdateAsync_WhenTeamExists_UpdatesTeam()
    {
        // Arrange
        var existingTeam = new Team { Id = 1, Name = "Old Name", Season = "2024" };
        var dto = new CreateTeamDto { Name = "New Name", Season = "2025" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingTeam);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Team>())).Returns(Task.CompletedTask);

        // Act
        await _teamService.UpdateAsync(1, dto);

        // Assert - verify update was called with the changed team
        _mockRepo.Verify(r => r.UpdateAsync(It.Is<Team>(t =>
            t.Name == "New Name" && t.Season == "2025")), Times.Once);
    }

    // checks that Delete calls the repo with the correct id
    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        // Arrange
        _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        await _teamService.DeleteAsync(1);

        // Assert - verify delete was called with the right id
        _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}
