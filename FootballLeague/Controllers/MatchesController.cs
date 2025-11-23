using FootballLeague.Data.DTOs.Matches;
using FootballLeague.Data.DTOs.Teams;
using FootballLeague.Interfaces;
using FootballLeague.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeague.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class MatchesController : ControllerBase
    {
        private IMatchesService matchService;

        public MatchesController(IMatchesService matchService)
        {
            this.matchService = matchService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamResponseDto>>> GetMatches()
        {
            var matches = await this.matchService.GetMatchesAsync();
            return Ok(matches.Value);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TeamResponseDto>> GetMatchById(string id)
        {
            var match = await this.matchService.GetMatchByIdAsync(id);

            if (!match.IsSuccess) { return NotFound(match.Message); }

            return Ok(match.Value);
        }

        [HttpPost]
        public async Task<ActionResult<TeamResponseDto>> CreateMatch([FromBody] MatchCreateDto dto)
        {
            if (dto.HomeTeamName == dto.AwayTeamName) return BadRequest("Home team name and Away team name should be different.");

            var response = await this.matchService.CreateMatchAsync(dto);

            if (!response.IsSuccess) { return BadRequest(response.Message); }

            return CreatedAtAction(nameof(GetMatches), new { id = response.Value.Id }, response);
        }

        [HttpPut]

        public async Task<ActionResult<TeamResponseDto>> UpdateMatch(string matchId, [FromBody] MatchUpdateDto dto)
        {
            if (dto.HomeTeamName == dto.AwayTeamName) return BadRequest("Home team name and Away team name should be different.");

            var response = await this.matchService.UpdateMatchAsync(matchId, dto);

            if (!response.IsSuccess) return BadRequest(response.Message);

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteMatch(string id)
        {
            var response = await this.matchService.DeleteMatchAsync(id);

            if (!response.IsSuccess) return BadRequest(response.Message);

            return NoContent();
        }

    }
}
