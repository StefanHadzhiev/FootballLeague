using System.ComponentModel.DataAnnotations;

namespace FootballLeague.Data.DTOs.Teams
{
    public class TeamCreateDto
    {
        [Required]
        public string Name { get; set; }
    }
}
