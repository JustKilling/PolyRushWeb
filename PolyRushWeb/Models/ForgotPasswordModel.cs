using System.ComponentModel.DataAnnotations;

namespace PolyRushWeb.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
    public class ResetPasswordModel
    {
        [Required]
        [RegularExpression("^.*(?=.*?[0-9]).{7,}$", ErrorMessage ="Please make sure the password is 8 characters long and has at least 1 digit.")]
        public string NewPassword { get; set; }
        [Compare("NewPassword", ErrorMessage = "Those passwords didn't match. Try again.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
