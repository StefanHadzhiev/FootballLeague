namespace FootballLeague.Data.DTOs.Matches
{
    public class MatchUpdateDto
    {
        public string HomeTeamName { get; set; }
        public string AwayTeamName { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public DateTime PlayedOn { get; set; }
    }
}
