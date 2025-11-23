

using FootballLeague.Data;
using FootballLeague.Data.DTOs.Rankings;
using FootballLeague.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeague.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankingController : ControllerBase
    {
        private IRankingService rankingService;
        public RankingController(IRankingService rankingService)
        {
            this.rankingService = rankingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RankingResponseDto>>> GetRankings()
        {
            var rankings = await this.rankingService.GetRankingsAsync();
            return Ok(rankings.Value);
        }
    }
}
