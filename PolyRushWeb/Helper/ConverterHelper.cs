using PolyRushWeb.Models;

namespace PolyRushWeb.Helper
{
    public static class ConverterHelper
    {
        public static UserDTO ToUserDTO(this User user)
        {
            return new()
            {
                Username = user.UserName,
                Avatar = user.Avatar,
                Coins = user.Coins,
                Coinsgathered = user.Coinsgathered,
                Coinsspent = user.Coinsspent,
                Email = user.Email,
                Firstname = user.Firstname,
                Highscore = user.Highscore,
                ID = user.Id,
                IsAdmin = user.IsAdmin,
                Itemspurchased = user.Itemspurchased,
                Lastname = user.Lastname,
                SeesAds = user.SeesAds,
                Timespassed = user.Timespassed
            };
        }
    }
}