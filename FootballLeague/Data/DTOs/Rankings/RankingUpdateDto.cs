namespace FootballLeague.Data.DTOs.Rankings
{
    public class RankingUpdateDto
    {
        public string TeamName { get; set; }
        public int Points { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Draws { get; set; } = 0;
        public int Losses { get; set; } = 0; 
    }
}
