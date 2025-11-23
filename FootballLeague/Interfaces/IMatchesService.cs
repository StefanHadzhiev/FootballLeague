using FootballLeague.Data;
using FootballLeague.Data.DTOs.Matches;
using FootballLeague.Data.DTOs.Teams;
using FootballLeague.Shared.Helpers;

namespace FootballLeague.Interfaces
{
    public interface IMatchesService
    {
        public Task<Result<IList<MatchResponseDto>>> GetMatchesAsync();

        public Task<Result<MatchResponseDto>> GetMatchByIdAsync(string id);

        public Task<Result<MatchResponseDto>> CreateMatchAsync(MatchCreateDto team);

        public Task<Result<bool>> UpdateMatchAsync(string matchId, MatchUpdateDto team);

        public Task<Result<bool>> DeleteMatchAsync(string id);
    }
}
