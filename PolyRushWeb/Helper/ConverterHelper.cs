using PolyRushWeb.Models;

namespace PolyRushWeb.Helper
{
    public static class ConverterHelper
    {
        public static UserDTO ToUserDTO(this User user)
        {
            return new UserDTO
            {
                Username = user.UserName,
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
                Timespassed = user.Timespassed,
                IsActive = user.IsActive
            };
        }
        public static User ToUser(this UserDTO user)
        {
            return new User
            {
                UserName = user.Username,
                Coins = user.Coins,
                Coinsgathered = user.Coinsgathered,
                Coinsspent = user.Coinsspent,
                Email = user.Email,
                Firstname = user.Firstname,
                Highscore = user.Highscore,
                Id = user.ID,
                IsAdmin = user.IsAdmin,
                Itemspurchased = user.Itemspurchased,
                Lastname = user.Lastname,
                SeesAds = user.SeesAds,
                Timespassed = user.Timespassed,
                IsActive = user.IsActive
            };
        }

        public static UserEditAdminModel ToUserEditAdminModel(this User user)
        {
            return new UserEditAdminModel
            {
                Coins = user.Coins,
                Coinsgathered = user.Coinsgathered,
                Coinsspent = user.Coinsspent,
                Email = user.Email,
                Firstname = user.Firstname,
                Highscore = user.Highscore,
                Id = user.Id,
                IsAdmin = user.IsAdmin,
                //if null --> true
                IsActive = user.IsActive,
                Itemspurchased = user.Itemspurchased,
                Lastname = user.Lastname,
                Scoregathered = user.Scoregathered,
                //if null --> true
                SeesAds = user.SeesAds ?? true,
                Timespassed = user.Timespassed,
                Username = user.UserName,
            };
        }
    }
}