using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PolyRushWeb.Helper;

namespace PolyRushWeb.Models
{
    public class UserEditModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:20, MinimumLength = 3, ErrorMessage = "Please provide a length between 3 and 20")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please provide a valid email adress!")]
        public string Email { get; set; }
        [Required]
        [StringLength(maximumLength:20, MinimumLength = 3, ErrorMessage = "Please provide a length between 3 and 20")]
        public string Firstname { get; set; }
        [Required]
        [StringLength(maximumLength:20, MinimumLength = 3, ErrorMessage = "Please provide a length between 3 and 20")]
        public string Lastname { get; set; }

        [DisplayName("Upload file")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile? Image { get; set; }
    }
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
        [Required]
        public bool IsActive { get; set; }
    }

}
