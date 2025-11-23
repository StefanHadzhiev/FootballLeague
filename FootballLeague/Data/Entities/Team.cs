


using System.ComponentModel.DataAnnotations;

namespace FootballLeague.Data.Entities
{
    public class Team
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MinLength(1, ErrorMessage = "Name cannot be empty.")]
        public string Name { get; set; }
        public ICollection<Match> HomeMatches { get; set; } = new List<Match>();
        public ICollection<Match> AwayMatches { get; set; } = new List<Match>();
        public int Wins { get; set; } = 0; 

        public int Draws { get; set; } = 0;

        public int Losses { get; set; } = 0;

        public int MatchesPlayed { get; set; } = 0;
    }
}
