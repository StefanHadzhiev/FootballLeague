using FootballLeague.Data;
using FootballLeague.Data.DTOs.Teams;
using FootballLeague.Shared.Helpers;

namespace FootballLeague.Interfaces
{
    public interface ITeamsService
    {

        public Task<Result<List<TeamResponseDto>>> GetTeamsAsync();

        public Task<Result<TeamResponseDto>> GetTeamByNameAsync(string name);

        public Task<Result<TeamResponseDto>> CreateTeamAsync(TeamCreateDto team);

        public Task<Result<bool>> UpdateTeamAsync(string teamName, TeamUpdateDto team);

        public Task<Result<bool>> DeleteTeamAsync(string name);
    }
}
