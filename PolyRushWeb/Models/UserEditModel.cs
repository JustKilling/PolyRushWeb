using System.ComponentModel.DataAnnotations;

namespace PolyRushWeb.Models
{
    public class UserEditAdminModel
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public bool SeesAds { get; set; }
        public int Coins { get; set; }
        public int Highscore { get; set; }
        public int Scoregathered { get; set; }
        public int Itemspurchased { get; set; }
        public int Coinsspent { get; set; }
        public int Coinsgathered { get; set; }
        public int Timespassed { get; set; }
        public bool? IsActive { get; set; }
    }
}
