using System.Security.Principal;

namespace FootballLeague.Data.DTOs.Matches
{
    public class MatchResponseDto
    {
        public Guid Id { get; set; }
        public Guid HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public Guid AwayTeamId { get; set; }
        public string AwayTeamName { get; set; }
        public int HomeScore { get; set; } 
        public int AwayScore { get; set; }
        public string PlayedOn { get; set; }
    }
}
