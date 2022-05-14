using Microsoft.AspNetCore.Identity;

namespace PolyRushWeb.Models
{
    public class User : IdentityUser<int>
    {
        public User()
        {
        }

        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public bool? SeesAds { get; set; }
        public int Coins { get; set; }
        public int Highscore { get; set; }
        public int Scoregathered { get; set; }
        public int Itemspurchased { get; set; }
        public int Coinsspent { get; set; }
        public int Coinsgathered { get; set; }
        public int Timespassed { get; set; }
        public DateTime LastLoginTime { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

    }
}
