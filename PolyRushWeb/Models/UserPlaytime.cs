using Microsoft.AspNetCore.Identity;
using PolyRushWeb.DA;
using System.Security.Claims;

namespace PolyRushWeb.Models
{
    public class UserPlaytime
    {
        public UserDTO User { get; set; } = null!;
        public TimeSpan Playtime { get; set; }
    }
}
