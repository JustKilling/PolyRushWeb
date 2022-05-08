using System.ComponentModel.DataAnnotations;

namespace PolyRushWeb.Models
{
    public class UserEditAdminModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Firstname { get; set; } = null!;
        [Required]
        public string Lastname { get; set; } = null!;
        [Required]
        public bool IsAdmin { get; set; }
        public bool SeesAds { get; set; }
        [Required]
        public int Coins { get; set; }
        [Required]
        public int Highscore { get; set; }
        [Required]
        public int Scoregathered { get; set; }
        [Required]
        public int Itemspurchased { get; set; }
        [Required]
        public int Coinsspent { get; set; }
        [Required]
        public int Coinsgathered { get; set; }
        [Required]
        public int Timespassed { get; set; }
        public bool IsActive { get; set; }
    }
}
