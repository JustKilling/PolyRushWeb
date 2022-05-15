
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PolyRushWeb.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
    public class RegisterModel
    {
        [Required(ErrorMessage = "Please provide a username")]
        [RegularExpression("^[A-Za-z0-9_-]{2,15}$", ErrorMessage ="Please provide a username between 3 and 15 characters. No special characters allowed.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please provide your first name")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Please provide your last name")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Please provide an active email-adress.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please provide a password.")]
        [RegularExpression("^.*(?=.*?[0-9]).{7,}$", ErrorMessage ="Please make sure the password is 8 characters long and has at least 1 digit.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please provide a matching password to verify.")]
        [Compare("Password", ErrorMessage ="Those passwords didn't match. Try again.")]
        public string ConfirmPassword { get; set; }
        public string? Avatar { get; set; }
    }
    
}
