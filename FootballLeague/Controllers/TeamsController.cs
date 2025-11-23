using FootballLeague.Data;
using FootballLeague.Data.DTOs.Teams;
using FootballLeague.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Diagnostics;

namespace FootballLeague.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private ITeamsService teamService;

        public TeamsController(ApplicationDbContext appDbContext, ITeamsService teamsService)
        {
            this.teamService = teamsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamResponseDto>>> GetTeams()
        {
            //Works fine
            var teams = await teamService.GetTeamsAsync();
            return Ok(teams.Value);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<TeamResponseDto>> GetTeamByName(string name)
        {
            //Works fine

            var team = await this.teamService.GetTeamByNameAsync(name);

            if(!team.IsSuccess) { return NotFound(team.Message); }

            return Ok(team.Value);
        }

        [HttpPost]
        public async Task<ActionResult<TeamResponseDto>> CreateTeam([FromBody] TeamCreateDto dto)
        {
            //Works fine

            var response = await this.teamService.CreateTeamAsync(dto);

            if (!response.IsSuccess) { return BadRequest(response.Message); }

            return CreatedAtAction(nameof(GetTeams), new { id = response.Value.Id }, response.Value);
        }

        [HttpPut("{teamName}")]
        
        public async Task<ActionResult<TeamResponseDto>> UpdateTeam(string teamName, [FromBody] TeamUpdateDto dto)
        {
            var response = await this.teamService.UpdateTeamAsync(teamName, dto);

            if (!response.IsSuccess) return BadRequest(response.Message);

            return NoContent();
        }

        [HttpDelete("delete/{name}")]
        public async Task<ActionResult> DeleteTeam(string name)
        {
            var response = await this.teamService.DeleteTeamAsync(name);

            if (!response.IsSuccess) return BadRequest(response.Message);

            return NoContent();
        }
    }
}
