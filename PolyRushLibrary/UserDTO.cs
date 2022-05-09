namespace PolyRushLibrary
{
    public class UserDTO
    {
        public int ID;
        public string Avatar;
        public int Coins;
        public int Coinsgathered;
        public int Coinsspent;
        public string Email;
        public string Firstname;
        public int Highscore;
        public bool IsAdmin;
        public int Itemspurchased;
        public string Lastname;
        public bool? SeesAds;
        public int Timespassed;
        public string Username;
        public bool IsActive;

        ////implicit operator to conver user to userdto
        //public static implicit operator UserDTO(ApplicationUser user)
        //{
        //    return new UserDTO
        //    {
        //        ID = user.IDUser,
        //        Avatar = user.Avatar.ToString(),
        //        Coinsgathered = user.Coinsgathered,
        //        Coinsspent = user.Coinsspent,
        //        Email = user.Email,
        //        Firstname = user.Firstname,
        //        Lastname = user.Lastname,
        //        Username = user.Username,
        //        Highscore = user.Highscore,
        //        IsAdmin = user.IsAdmin,
        //        Itemspurchased = user.Itemspurchased,
        //        Coins = user.Coins
        //    };
        //}
    }
}