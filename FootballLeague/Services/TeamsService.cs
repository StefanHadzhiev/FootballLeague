using FootballLeague.Data;
using FootballLeague.Data.DTOs.Rankings;
using FootballLeague.Data.DTOs.Teams;
using FootballLeague.Data.Entities;
using FootballLeague.Shared.Helpers;
using FootballLeague.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Xml.Linq;
using FootballLeague.Shared;

namespace FootballLeague.Services
{
    public class TeamsService : ITeamsService
    {
        public ApplicationDbContext context { get; set; }

        public TeamsService(ApplicationDbContext appDbContext)
        {
            this.context = appDbContext;
        }
        public async Task<Result<List<TeamResponseDto>>> GetTeamsAsync()
        {
            var teams = await this.context.Teams.ToListAsync();

            var response = teams.Select(t => new TeamResponseDto()
            {
                Id = t.Id,
                Name = t.Name,
            }).ToList();

            return Result<List<TeamResponseDto>>.Success(response, String.Format(Constants.GetSuccessMessage, "Teams"));
        }
        public async Task<Result<TeamResponseDto>> GetTeamByNameAsync(string name)
        {
            var team = await this.context.Teams.FirstOrDefaultAsync(t => t.Name == name);

            if (team == null) return Result<TeamResponseDto>.Failure(String.Format(Constants.InvalidNameErrorMessage, "Team"));

            var response = new TeamResponseDto()
            {
                Id = team.Id,
                Name = team.Name,
                Wins = team.Wins,
                Losses = team.Losses, 
                Draws = team.Draws,
                MatchesPlayed = team.MatchesPlayed
            };

            return Result<TeamResponseDto>.Success(response, String.Format(Constants.GetSuccessMessage, "Team"));
        }
        public async Task<Result<TeamResponseDto>> CreateTeamAsync(TeamCreateDto dto)
        {
            // validate team does not exist 
            var team = await this.context.Teams.FirstOrDefaultAsync(t => t.Name == dto.Name);
            // if there is a team with this name ,return null (proper error handling)
            if (team != null) return Result<TeamResponseDto>.Failure(String.Format(Constants.EntityAlreadyExistsErrorMessage, "Team", "name"));

            var teamEntity = new Team()
            {
                Name = dto.Name,
            };

            this.context.Teams.Add(teamEntity);
            await context.SaveChangesAsync();

            TeamResponseDto response = new TeamResponseDto()
            {
                Id = teamEntity.Id,
                Name = teamEntity.Name,
            };

            return Result<TeamResponseDto>.Success(response, String.Format(Constants.PostSuccessMessage, "Team")); 
        }

        public async Task<Result<bool>> UpdateTeamAsync(string teamName, TeamUpdateDto dto)
        {
            var team = await context.Teams.FirstOrDefaultAsync(t => t.Name == teamName);

            if (team == null) return Result<bool>.Failure(String.Format(Constants.InvalidNameErrorMessage, "Team"));

            team.Name = dto.Name;
            team.Wins = dto.Wins;
            team.Draws = dto.Draws;
            team.Losses = dto.Losses;
            team.MatchesPlayed = dto.MatchesPlayed;

            await this.context.SaveChangesAsync();

            return Result<bool>.Success(true, String.Format(Constants.UpdateSuccessMessage, "Team"));
        }

        public async Task<Result<bool>> DeleteTeamAsync(string name)
        {
            var team = await context.Teams.FirstOrDefaultAsync(t => t.Name == name);

            // no team with such name exists
            if (team == null) return Result<bool>.Failure(String.Format(Constants.InvalidNameErrorMessage, "Team"));

            // this.rankingService.DeleteRankingAsync(team.Ranking.Id);

            this.context.Teams.Remove(team);
            this.context.SaveChanges();


            return Result<bool>.Success(true, String.Format(Constants.DeleteSuccessMessage, "Team")); 
        }
      
    }
}
