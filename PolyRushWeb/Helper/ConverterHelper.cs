using PolyRushWeb.Models;

namespace PolyRushWeb.Helper
{
    public static class ConverterHelper
    {
        //extension methods
        //method to convert a user to a userDTO
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
                Itemspurchased = user.Itemspurchased,
                Lastname = user.Lastname,
                Timespassed = user.Timespassed,
                IsActive = user.IsActive
            };
        }
        //method to convert a userDTO to a user
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
                Itemspurchased = user.Itemspurchased,
                Lastname = user.Lastname,
                Timespassed = user.Timespassed,
                IsActive = user.IsActive
            };
        }
        //method to convert a user to a usereditadminmodel
        public static UserEditAdminModel ToUserEditAdminModel(this User user, bool isAdmin = false)
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
                IsAdmin = isAdmin,
                //if null --> true
                IsActive = user.IsActive,
                Itemspurchased = user.Itemspurchased,
                Lastname = user.Lastname,
                Scoregathered = user.Scoregathered,
                Timespassed = user.Timespassed,
                Username = user.UserName,
            };
        }
        //method to convert a discount to a discountmodel
        public static DiscountModel ToDiscountModel(this Discount discount)
        {
            return new DiscountModel
            {
                
                ItemId = discount.ItemId,
                DiscountPercentage = discount.DiscountPercentage,
                Enddate = discount.Enddate,
                Startdate = discount.Startdate
            };
        }
        //method to convert a discountmodel to a discount
        public static Discount ToDiscount(this DiscountModel discount)
        {
            return new Discount
            {
              
                ItemId = discount.ItemId,
                DiscountPercentage = discount.DiscountPercentage,
                Enddate = discount.Enddate,
                Startdate = discount.Startdate
            };
        }
    }
}