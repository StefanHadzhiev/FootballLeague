using FootballLeague.Data;
using FootballLeague.Data.DTOs.Rankings;
using FootballLeague.Data.DTOs.Teams;
using FootballLeague.Data.Entities;
using FootballLeague.Interfaces;
using FootballLeague.Shared;
using FootballLeague.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FootballLeague.Services
{
    public class RankingService : IRankingService
    {

        ApplicationDbContext context { get; set; }

        public RankingService(ApplicationDbContext dbContext, ITeamsService teamsService)
        {
            this.context = dbContext;
        }
        public async Task<Result<List<RankingResponseDto>>> GetRankingsAsync()
        {
            var rankings = await context.Teams
            .Select(t => new RankingResponseDto
            {
                TeamName = t.Name,
                Wins = t.Wins,
                Draws = t.Draws,
                Losses = t.Losses,
                MatchesPlayed = t.MatchesPlayed,
                Points = (t.Wins * Constants.WinPoints) + (t.Draws * Constants.DrawPoints)
            })
            .OrderByDescending(r => r.Points)
            .ThenByDescending(r => r.Wins)
            .ThenBy(r => r.TeamName)
            .ToListAsync();

            return Result<List<RankingResponseDto>>.Success(rankings, String.Format(Constants.GetSuccessMessage, "Rankings"));
        }

    }
}
