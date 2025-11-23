using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using System.Security.Principal;

namespace FootballLeague.Data.Entities
{
    public class Match
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid HomeTeamId { get; set; }
        public Team HomeTeam { get; set; }

        public string HomeTeamName { get; set; }

        public Guid AwayTeamId { get; set; }
        public Team AwayTeam { get; set; }

        public string AwayTeamName { get; set; }

        public int HomeScore { get; set; }

        public int AwayScore { get; set; }

        public DateTime PlayedOn { get; set; } = DateTime.Now;
    }
}
