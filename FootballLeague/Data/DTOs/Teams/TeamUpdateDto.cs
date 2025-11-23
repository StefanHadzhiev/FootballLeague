using System.ComponentModel.DataAnnotations;

namespace FootballLeague.Data.DTOs.Teams
{
    public class TeamUpdateDto
    {
        [Required]
        public string Name { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int MatchesPlayed { get; set; }
    }

}
