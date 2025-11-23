using FootballLeague.Data;
using FootballLeague.Data.DTOs.Rankings;
using FootballLeague.Data.DTOs.Teams;
using FootballLeague.Shared.Helpers;

namespace FootballLeague.Interfaces
{
    public interface IRankingService
    {

        public Task<Result<List<RankingResponseDto>>> GetRankingsAsync();
    }
}
