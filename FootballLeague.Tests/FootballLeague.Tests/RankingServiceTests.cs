using FootballLeague.Data;
using FootballLeague.Data.Entities;
using FootballLeague.Services;
using Microsoft.EntityFrameworkCore;

namespace FootballLeague.Tests
{
    public class RankingServiceTests
    {
        private ApplicationDbContext _context;
        private RankingService _rankingService;



        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "FootballLeagueTestDb")
                .Options;

            _context = new ApplicationDbContext(options);

            // Ensure DB is clean for each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _rankingService = new RankingService(_context, null);
        }

        private void SeedTeams()
        {
            var teams = new List<Team>
            {
                new Team { Id = Guid.NewGuid(), Name = "CSKA", Wins = 5, Draws = 1, Losses = 2, MatchesPlayed = 8 },
                new Team { Id = Guid.NewGuid(), Name = "Levski", Wins = 5, Draws = 1, Losses = 2, MatchesPlayed = 8 },
                new Team { Id = Guid.NewGuid(), Name = "Beroe", Wins = 4, Draws = 3, Losses = 1, MatchesPlayed = 8 },
                new Team { Id = Guid.NewGuid(), Name = "Slavia", Wins = 3, Draws = 3, Losses = 2, MatchesPlayed = 8 }
            };

            _context.Teams.AddRange(teams);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetRankingsAsync_ShouldReturnSuccessResult()
        {
            SeedTeams();

            var result = await _rankingService.GetRankingsAsync();

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(4, result.Value.Count);
            Assert.AreEqual(result.Value[0].TeamName, "CSKA");
        }

        [Test]
        public async Task GetRankingsAsync_ShouldCalculateCorrectPoints()
        {
            SeedTeams();

            var result = await _rankingService.GetRankingsAsync();
            var cska = result.Value.Find(t => t.TeamName == "CSKA");

            var expectedPoints = (5 * FootballLeague.Shared.Constants.WinPoints) +
                                 (1 * FootballLeague.Shared.Constants.DrawPoints);

            Assert.AreEqual(expectedPoints, cska.Points);
        }

        [Test]
        public async Task GetRankingsAsync_ShouldOrderByPointsThenWinsThenName()
        {
            SeedTeams();

            var result = await _rankingService.GetRankingsAsync();
            var rankings = result.Value;

            Assert.AreEqual("CSKA", rankings[0].TeamName);  // CSKA and Levski tied, CSKA comes first
            Assert.AreEqual("Levski", rankings[1].TeamName);
            Assert.AreEqual("Beroe", rankings[2].TeamName);
            Assert.AreEqual("Slavia", rankings[3].TeamName);
        }

        [Test]
        public async Task GetRankingsAsync_WhenNoTeams_ReturnsEmptyList()
        {
            var result = await _rankingService.GetRankingsAsync();

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, result.Value.Count);
        }

    }

}
