using FootballLeague.Data;
using FootballLeague.Data.DTOs.Teams;
using FootballLeague.Data.Entities;
using FootballLeague.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballLeague.Tests
{
    public class TeamsServiceTests
    {
        private ApplicationDbContext _context;
        private TeamsService _teamsService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TeamsServiceTestDb")
                .Options;

            _context = new ApplicationDbContext(options);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _teamsService = new TeamsService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void SeedTeams()
        {
            var teams = new List<Team>
            {
                new() { Id = Guid.NewGuid(), Name = "Levski", Wins = 2, Draws = 1, Losses = 1, MatchesPlayed = 4 },
                new() { Id = Guid.NewGuid(), Name = "CSKA", Wins = 3, Draws = 0, Losses = 1, MatchesPlayed = 4 }
            };

            _context.Teams.AddRange(teams);
            _context.SaveChanges();
        }

        [Test]
        public async Task GetTeamsAsync_ShouldReturnAllTeams()
        {
            SeedTeams();

            var result = await _teamsService.GetTeamsAsync();

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Value.Count);
            Assert.IsTrue(result.Value.Any(t => t.Name == "CSKA"));
            Assert.IsTrue(result.Value.Any(t => t.Name == "Levski"));
        }

        [Test]
        public async Task GetTeamByNameAsync_WhenExists_ShouldReturnTeam()
        {
            SeedTeams();

            var result = await _teamsService.GetTeamByNameAsync("CSKA");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("CSKA", result.Value.Name);
            Assert.AreEqual(3, result.Value.Wins);
        }

        [Test]
        public async Task GetTeamByNameAsync_WhenNotExists_ShouldFail()
        {
            var result = await _teamsService.GetTeamByNameAsync("Dolna Malina");

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Value);
        }

        [Test]
        public async Task CreateTeamAsync_ShouldCreateTeamSuccessfully()
        {
            var dto = new TeamCreateDto { Name = "Nov Otbor" };

            var result = await _teamsService.CreateTeamAsync(dto);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Nov Otbor", result.Value.Name);

            var exists = _context.Teams.Any(t => t.Name == "Nov Otbor");
            Assert.IsTrue(exists);
        }

        [Test]
        public async Task CreateTeamAsync_WhenNameExists_ShouldFail()
        {
            SeedTeams();

            var dto = new TeamCreateDto { Name = "CSKA" };

            var result = await _teamsService.CreateTeamAsync(dto);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Value);
        }

        [Test]
        public async Task UpdateTeamAsync_WhenExists_ShouldUpdateTeam()
        {
            SeedTeams();

            var dto = new TeamUpdateDto
            {
                Name = "CSKARenamed",
                Wins = 10,
                Draws = 5,
                Losses = 2,
                MatchesPlayed = 17
            };

            var result = await _teamsService.UpdateTeamAsync("CSKA", dto);

            Assert.IsTrue(result.IsSuccess);

            var updated = _context.Teams.First(t => t.Name == "CSKARenamed");

            Assert.AreEqual(10, updated.Wins);
            Assert.AreEqual(5, updated.Draws);
            Assert.AreEqual(17, updated.MatchesPlayed);
        }

        [Test]
        public async Task UpdateTeamAsync_WhenNotExists_ShouldFail()
        {
            var dto = new TeamUpdateDto { Name = "X" };

            var result = await _teamsService.UpdateTeamAsync("Dolna Malina", dto);

            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task DeleteTeamAsync_WhenExists_ShouldDeleteTeam()
        {
            SeedTeams();

            var result = await _teamsService.DeleteTeamAsync("CSKA");

            Assert.IsTrue(result.IsSuccess);

            Assert.IsFalse(_context.Teams.Any(t => t.Name == "CSKA"));
        }

        [Test]
        public async Task DeleteTeamAsync_WhenNotExists_ShouldFail()
        {
            var result = await _teamsService.DeleteTeamAsync("Dolno Nagornishte FC");

            Assert.IsFalse(result.IsSuccess);
        }
    }
}
