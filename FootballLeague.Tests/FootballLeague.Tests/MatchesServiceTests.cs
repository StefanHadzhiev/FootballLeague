using FootballLeague.Data;
using FootballLeague.Data.DTOs.Matches;
using FootballLeague.Data.DTOs.Teams;
using FootballLeague.Interfaces;
using FootballLeague.Services;
using FootballLeague.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FootballLeague.Tests
{
    public class MatchesServiceTests
    {
        private ApplicationDbContext _context;
        private MatchesService _matchesService;
        private Mock<ITeamsService> _mockTeamsService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("MatchesServiceTestsDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _mockTeamsService = new Mock<ITeamsService>();

            _matchesService = new MatchesService(_context, _mockTeamsService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void SeedMatches()
        {
            var matches = new List<FootballLeague.Data.Entities.Match>
            {
                new FootballLeague.Data.Entities.Match
                {
                    Id = Guid.NewGuid(),
                    HomeTeamId = Guid.NewGuid(),
                    HomeTeamName = "CSKA",
                    HomeScore = 2,
                    AwayTeamId = Guid.NewGuid(),
                    AwayTeamName = "Levski",
                    AwayScore = 1,
                    PlayedOn = DateTime.UtcNow
                }
            };

            _context.Matches.AddRange(matches);
            _context.SaveChanges();
        }

        private Result<TeamResponseDto> FakeTeam(string name)
        {
            return Result<TeamResponseDto>.Success(new TeamResponseDto
            {
                Id = Guid.NewGuid(),
                Name = name,
                Wins = 0,
                Draws = 0,
                Losses = 0,
                MatchesPlayed = 0
            }, "");
        }

        [Test]
        public async Task GetMatchesAsync_ShouldReturnAllMatches()
        {
            SeedMatches();

            var result = await _matchesService.GetMatchesAsync();

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value.Count);
            Assert.AreEqual("CSKA", result.Value[0].HomeTeamName);
        }

        [Test]
        public async Task GetMatchByIdAsync_WhenExists_ShouldReturnMatch()
        {
            SeedMatches();
            var match = _context.Matches.First();

            var result = await _matchesService.GetMatchByIdAsync(match.Id.ToString());

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(match.Id, result.Value.Id);
        }

        [Test]
        public async Task GetMatchByIdAsync_WhenNotExists_ShouldFail()
        {
            var result = await _matchesService.GetMatchByIdAsync(Guid.NewGuid().ToString());

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Value);
        }

        [Test]
        public async Task CreateMatchAsync_WhenValid_ShouldCreateMatch()
        {
            // Arrange
            _mockTeamsService.Setup(s => s.GetTeamByNameAsync("Alpha"))
                .ReturnsAsync(FakeTeam("Alpha"));

            _mockTeamsService.Setup(s => s.GetTeamByNameAsync("Bravo"))
                .ReturnsAsync(FakeTeam("Bravo"));

            _mockTeamsService.Setup(s => s.UpdateTeamAsync(It.IsAny<string>(), It.IsAny<TeamUpdateDto>()))
                .ReturnsAsync(Result<bool>.Success(true, "Successfully updated Team."));

            var dto = new MatchCreateDto
            {
                HomeTeamName = "Alpha",
                HomeScore = 3,
                AwayTeamName = "Bravo",
                AwayScore = 1
            };

            // Act
            var result = await _matchesService.CreateMatchAsync(dto);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, _context.Matches.Count());

            _mockTeamsService.Verify(s =>
                s.UpdateTeamAsync("Alpha", It.IsAny<TeamUpdateDto>()), Times.Once);

            _mockTeamsService.Verify(s =>
                s.UpdateTeamAsync("Bravo", It.IsAny<TeamUpdateDto>()), Times.Once);
        }

        [Test]
        public async Task CreateMatchAsync_WhenHomeTeamInvalid_ShouldFail()
        {
            _mockTeamsService.Setup(s => s.GetTeamByNameAsync("Alpha"))
                .ReturnsAsync(Result<TeamResponseDto>.Failure("error"));

            var dto = new MatchCreateDto
            {
                HomeTeamName = "Alpha",
                AwayTeamName = "Bravo"
            };

            var result = await _matchesService.CreateMatchAsync(dto);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(0, _context.Matches.Count());
        }

        [Test]
        public async Task CreateMatchAsync_WhenAwayTeamInvalid_ShouldFail()
        {
            _mockTeamsService.Setup(s => s.GetTeamByNameAsync("Alpha"))
                .ReturnsAsync(FakeTeam("Alpha"));

            _mockTeamsService.Setup(s => s.GetTeamByNameAsync("Bravo"))
                .ReturnsAsync(Result<TeamResponseDto>.Failure("error"));

            var dto = new MatchCreateDto
            {
                HomeTeamName = "Alpha",
                AwayTeamName = "Bravo"
            };

            var result = await _matchesService.CreateMatchAsync(dto);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(0, _context.Matches.Count());
        }

        [Test]
        public async Task UpdateMatchAsync_WhenValid_ShouldUpdate()
        {
            SeedMatches();
            var match = _context.Matches.First();

            _mockTeamsService.Setup(s => s.GetTeamByNameAsync("Alpha"))
                .ReturnsAsync(FakeTeam("Alpha"));

            _mockTeamsService.Setup(s => s.GetTeamByNameAsync("Bravo"))
                .ReturnsAsync(FakeTeam("Bravo"));

            var dto = new MatchUpdateDto
            {
                HomeTeamName = "Alpha",
                HomeScore = 5,
                AwayTeamName = "Bravo",
                AwayScore = 4,
                PlayedOn = DateTime.UtcNow
            };

            var result = await _matchesService.UpdateMatchAsync(match.Id.ToString(), dto);

            Assert.IsTrue(result.IsSuccess);
            var updated = _context.Matches.First();
            Assert.AreEqual(5, updated.HomeScore);
            Assert.AreEqual(4, updated.AwayScore);
        }

        [Test]
        public async Task UpdateMatchAsync_WhenIdInvalid_ShouldFail()
        {
            var dto = new MatchUpdateDto();

            var result = await _matchesService.UpdateMatchAsync(Guid.NewGuid().ToString(), dto);

            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task DeleteMatchAsync_WhenExists_ShouldDelete()
        {
            SeedMatches();
            var match = _context.Matches.First();

            var result = await _matchesService.DeleteMatchAsync(match.Id.ToString());

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, _context.Matches.Count());
        }

        [Test]
        public async Task DeleteMatchAsync_WhenNotExists_ShouldFail()
        {
            var result = await _matchesService.DeleteMatchAsync(Guid.NewGuid().ToString());

            Assert.IsFalse(result.IsSuccess);
        }
    }

}
