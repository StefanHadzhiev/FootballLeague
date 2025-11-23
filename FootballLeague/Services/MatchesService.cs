using FootballLeague.Data;
using FootballLeague.Data.DTOs.Matches;
using FootballLeague.Data.DTOs.Teams;
using FootballLeague.Data.Entities;
using FootballLeague.Interfaces;
using FootballLeague.Shared;
using FootballLeague.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FootballLeague.Services
{
    public class MatchesService : IMatchesService
    {
        public ApplicationDbContext context { get; set; }
        private ITeamsService teamsService { get; set; }

        public MatchesService(ApplicationDbContext appDbContext, ITeamsService teamsService)
        {
            context = appDbContext;
            this.teamsService = teamsService;
        }
        public async Task<Result<IList<MatchResponseDto>>> GetMatchesAsync()
        {
            var matches = await this.context.Matches.ToListAsync();

            var response = matches.Select(t => new MatchResponseDto()
            {
                Id = t.Id,

                HomeTeamId = t.HomeTeamId,
                HomeTeamName = t.HomeTeamName,
                HomeScore = t.HomeScore,

                AwayTeamId = t.AwayTeamId,
                AwayTeamName = t.AwayTeamName,
                AwayScore = t.AwayScore,

                PlayedOn = t.PlayedOn.ToString()
            }).ToList();

            return Result<IList<MatchResponseDto>>.Success(response, String.Format(Constants.GetSuccessMessage, "Matches"));
        }
        public async Task<Result<MatchResponseDto>> GetMatchByIdAsync(string id)
        {
            var res = GuidHelper.ValidateGuid(id, "Team");

            var match = await this.context.Matches.FirstOrDefaultAsync(t => t.Id == res);

            if (match == null) return Result<MatchResponseDto>.Failure(String.Format(Constants.InvalidIdErrorMessage, "Match"));

            var response = new MatchResponseDto()
            {
                Id = match.Id,

                HomeTeamId = match.HomeTeamId,
                HomeTeamName = match.HomeTeamName,
                HomeScore = match.HomeScore,

                AwayTeamId = match.AwayTeamId,
                AwayTeamName = match.AwayTeamName,
                AwayScore = match.AwayScore,

                PlayedOn = match.PlayedOn.ToString()
            };

            return Result<MatchResponseDto>.Success(response, String.Format(Constants.GetSuccessMessage, "Match"));
        }

        public async Task<Result<MatchResponseDto>> CreateMatchAsync(MatchCreateDto match)
        {
            var homeTeam = await this.teamsService.GetTeamByNameAsync(match.HomeTeamName);
            if (!homeTeam.IsSuccess) return Result<MatchResponseDto>.Failure(Constants.InvalidHomeTeamNameErrorMessage);

            var awayTeam = await this.teamsService.GetTeamByNameAsync(match.AwayTeamName);
            if (!awayTeam.IsSuccess) return Result<MatchResponseDto>.Failure(Constants.InvalidAwayTeamNameErrorMessage);

            var newMatch = new Match()
            {
                HomeTeamId = homeTeam.Value.Id,
                HomeTeamName = homeTeam.Value.Name,
                HomeScore = match.HomeScore,

                AwayTeamId = awayTeam.Value.Id,
                AwayTeamName = awayTeam.Value.Name,
                AwayScore = match.AwayScore,

                PlayedOn = DateTime.Now
            };

            await CreateStats(homeTeam.Value, awayTeam.Value, match);

            this.context.Matches.Add(newMatch);
            await this.context.SaveChangesAsync();

            var response = new MatchResponseDto()
            {
                Id = newMatch.Id,

                HomeTeamId = newMatch.HomeTeamId,
                HomeTeamName = newMatch.HomeTeamName,
                HomeScore = newMatch.HomeScore,

                AwayTeamId = newMatch.AwayTeamId,
                AwayTeamName = newMatch.AwayTeamName,
                AwayScore = newMatch.AwayScore,

                PlayedOn = newMatch.PlayedOn.ToString()
            };

            return Result<MatchResponseDto>.Success(response, String.Format(Constants.PostSuccessMessage, "Match"));
        }
        public async Task<Result<bool>> UpdateMatchAsync(string matchId, MatchUpdateDto dto)
        {
            var guid = GuidHelper.ValidateGuid(matchId, "Match");

            var match = this.context.Matches.FirstOrDefault(m => m.Id == guid);

            if (match == null) return Result<bool>.Failure(String.Format(Constants.InvalidIdErrorMessage, "Match"));

            var homeTeam = await this.teamsService.GetTeamByNameAsync(dto.HomeTeamName);
            var awayTeam = await this.teamsService.GetTeamByNameAsync(dto.AwayTeamName);

            var oldHomeScore = match.HomeScore;
            var oldAwayScore = match.AwayScore;

            match.HomeTeamId = homeTeam.Value.Id;
            match.HomeTeamName = homeTeam.Value.Name;
            match.HomeScore = dto.HomeScore;

            match.AwayTeamId = awayTeam.Value.Id;
            match.AwayTeamName = awayTeam.Value.Name;
            match.AwayScore = dto.AwayScore;

            match.PlayedOn = dto.PlayedOn;

            var matchDto = new MatchResponseDto()
            {
                Id = match.Id,
                HomeTeamId = match.HomeTeamId,
                HomeTeamName = match.HomeTeamName,
                HomeScore = match.HomeScore,
                AwayTeamId = match.AwayTeamId,
                AwayTeamName = match.AwayTeamName,
                AwayScore = match.AwayScore,
                PlayedOn = match.PlayedOn.ToString(),
            };

            await this.UpdateStats(homeTeam.Value, awayTeam.Value, oldHomeScore, oldAwayScore, matchDto);

            await this.context.SaveChangesAsync();

            return Result<bool>.Success(true, String.Format(Constants.UpdateSuccessMessage, "Match"));
        }
        public async Task<Result<bool>> DeleteMatchAsync(string id)
        {
            var guid = GuidHelper.ValidateGuid(id, "Match");

            var match = this.context.Matches.FirstOrDefault(m => m.Id == guid);

            if (match == null) return Result<bool>.Failure(String.Format(Constants.InvalidIdErrorMessage, "Match"));

            var homeTeam = await this.teamsService.GetTeamByNameAsync(match.HomeTeamName);
            var awayTeam = await this.teamsService.GetTeamByNameAsync(match.AwayTeamName);

            var matchDto = new MatchResponseDto()
            {
                Id = match.Id,
                HomeTeamId = match.HomeTeamId,
                HomeTeamName = match.HomeTeamName,
                HomeScore = match.HomeScore,
                AwayTeamId = match.AwayTeamId,
                AwayTeamName = match.AwayTeamName,
                AwayScore = match.AwayScore,
                PlayedOn = match.PlayedOn.ToString(),
            };

            await DeleteStats(homeTeam.Value, awayTeam.Value, matchDto);

            this.context.Matches.Remove(match);
            this.context.SaveChanges();

            return Result<bool>.Success(true, String.Format(Constants.DeleteSuccessMessage, "Match"));
        }

        private async Task CreateStats(TeamResponseDto homeTeam, TeamResponseDto awayTeam, MatchCreateDto match)
        {
            homeTeam.MatchesPlayed++;
            awayTeam.MatchesPlayed++;

            if (match.HomeScore > match.AwayScore)
            {
                homeTeam.Wins++;
                awayTeam.Losses++;
            }
            else if (match.HomeScore < match.AwayScore)
            {
                homeTeam.Losses++;
                awayTeam.Wins++;
            }
            else
            {
                homeTeam.Draws++;
                awayTeam.Draws++;
            }

            var homeTeamUpdateDto = new TeamUpdateDto()
            {
                Name = homeTeam.Name,
                Wins = homeTeam.Wins,
                Draws = homeTeam.Draws,
                Losses = homeTeam.Losses,
                MatchesPlayed = homeTeam.MatchesPlayed,
            };

            var awayTeamUpdateDto = new TeamUpdateDto()
            {
                Name = awayTeam.Name,
                Wins = awayTeam.Wins,
                Draws = awayTeam.Draws,
                Losses = awayTeam.Losses,
                MatchesPlayed = awayTeam.MatchesPlayed,
            };

            await this.teamsService.UpdateTeamAsync(homeTeam.Name, homeTeamUpdateDto);
            await this.teamsService.UpdateTeamAsync(awayTeam.Name, awayTeamUpdateDto);
        }

        private async Task UpdateStats(TeamResponseDto homeTeam, TeamResponseDto awayTeam, int oldHomeScore, int oldAwayScore, MatchResponseDto match)
        {
            // home team is winner again - nothing changes 
            if (oldHomeScore > oldAwayScore && match.HomeScore < match.AwayScore)
            {
                homeTeam.Wins--;
                homeTeam.Losses++;
                awayTeam.Wins++;
                awayTeam.Losses--;
            }
            else if (oldHomeScore > oldAwayScore && match.HomeScore == match.AwayScore)
            {
                homeTeam.Wins--;
                homeTeam.Draws++;
                awayTeam.Losses--;
                awayTeam.Draws++;
            }

            if (oldAwayScore > oldHomeScore && match.AwayScore < match.HomeScore)
            {
                awayTeam.Wins--;
                awayTeam.Losses++;
                homeTeam.Wins++;
                homeTeam.Losses--;
            }
            else if (oldAwayScore > oldHomeScore && match.AwayScore == match.HomeScore)
            {
                awayTeam.Wins--;
                awayTeam.Draws++;
                homeTeam.Losses--;
                homeTeam.Draws++;
            }

            if (oldAwayScore == oldHomeScore && match.AwayScore > match.HomeScore)
            {
                awayTeam.Draws--;
                homeTeam.Draws--;
                awayTeam.Wins++;
                homeTeam.Losses++;
            }

            else if (oldAwayScore == oldHomeScore && match.AwayScore < match.HomeScore)
            {
                awayTeam.Draws--;
                homeTeam.Draws--;
                awayTeam.Losses++;
                homeTeam.Wins++;
            }

            var homeTeamUpdateDto = new TeamUpdateDto()
            {
                Name = homeTeam.Name,
                Wins = homeTeam.Wins,
                Draws = homeTeam.Draws,
                Losses = homeTeam.Losses,
                MatchesPlayed = homeTeam.MatchesPlayed,
            };

            var awayTeamUpdateDto = new TeamUpdateDto()
            {
                Name = awayTeam.Name,
                Wins = awayTeam.Wins,
                Draws = awayTeam.Draws,
                Losses = awayTeam.Losses,
                MatchesPlayed = awayTeam.MatchesPlayed,
            };

            await this.teamsService.UpdateTeamAsync(homeTeam.Name, homeTeamUpdateDto);
            await this.teamsService.UpdateTeamAsync(awayTeam.Name, awayTeamUpdateDto);
        }

        private async Task DeleteStats(TeamResponseDto homeTeam, TeamResponseDto awayTeam, MatchResponseDto match)
        {
            if(match.HomeScore > match.AwayScore)
            {
                homeTeam.Wins--;
                awayTeam.Losses--;
            }
            else if(match.HomeScore < match.AwayScore)
            {
                homeTeam.Losses--;
                awayTeam.Wins--;
            } else
            {
                homeTeam.Draws--;
                awayTeam.Draws--;
            }

            var homeTeamUpdateDto = new TeamUpdateDto()
            {
                Name = homeTeam.Name,
                Wins = homeTeam.Wins,
                Draws = homeTeam.Draws,
                Losses = homeTeam.Losses,
                MatchesPlayed = homeTeam.MatchesPlayed,
            };

            var awayTeamUpdateDto = new TeamUpdateDto()
            {
                Name = awayTeam.Name,
                Wins = awayTeam.Wins,
                Draws = awayTeam.Draws,
                Losses = awayTeam.Losses,
                MatchesPlayed = awayTeam.MatchesPlayed,
            };

            await this.teamsService.UpdateTeamAsync(homeTeam.Name, homeTeamUpdateDto);
            await this.teamsService.UpdateTeamAsync(awayTeam.Name, awayTeamUpdateDto);
        }
    }
}
