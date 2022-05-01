using System.Data;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using PolyRushWeb.Helper;
using PolyRushWeb.Models;

namespace PolyRushWeb.DA
{
    public class UserDA
    {
        private readonly polyrushContext _context;
        private readonly UserManager<User> _userManager;

        public UserDA(polyrushContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<UserDTO>> GetUsers(bool getAvatar = true)
        {
            var users = new List<UserDTO>();
            if (getAvatar)
            {
                users = await _userManager.Users.Select(u => u.ToUserDTO()).ToListAsync()!;
            }
         
            return users;
        }

        public async Task<List<UserDTO>> GetUsers()
        {
            var users = new List<UserDTO>();
            users = await _userManager.Users.Select(u => u.ToUserDTO()).ToListAsync()!;
            return users;
        }

        public async Task Deactivate(int id, bool deactivate = true)
        {
            var user = new User
            {
                Id = id,
                IsActive = !deactivate
            };
            _context.Entry(user).Property(u => u.IsActive).IsModified = true;
            await _context.SaveChangesAsync();
        }

        public async Task<IdentityResult> AddUser(User user)
        {
            
            return IdentityResult.Success;
        }

        public  User? UserExists(string usernameoremail)
        {

            _context.Users.AnyAsync(u =>
                u.NormalizedEmail == usernameoremail.ToUpper() || u.NormalizedUserName == usernameoremail.ToUpper());
            return null;
        }

        public async Task<User?> GetById(int userId, bool getAvatar = true)
        {
            var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            return user;
        }

        public async Task<string?> GetAvatarById(int userId)
        {
            var avatar = await _context.Users.Where(u => u.Id == userId).Select(u => u.Avatar).FirstOrDefaultAsync();
            return avatar;
        }


        public async Task<bool> HasEnoughCoins(int id, int price)
        {
            return price < await GetCoinsAsync(id);
        }
        
        //Een methode om te kijken of dit een email is.
        private  bool IsEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public async Task<int> GetCoinsAsync(int id)
        {
            var coins = _context.Users;
            var coins2 = coins.Where(u => u.Id == id);
            var coins3 = coins2.Select(u => u.Coins);
            var coins4 = await coins3.FirstOrDefaultAsync();
                
            return coins4;
        }
        
        public async Task<bool> RemoveCoins(int id, int coins = -1)
        {
            int userCoinAmount = await GetCoinsAsync(id);
            if (userCoinAmount < coins) return false;
            
            var user = await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

            //if no coins have been given, remove all coins.
            int amount = coins <= 0 ? userCoinAmount : coins;
            user.Coins -= amount;

            return true;
        }
        
        public async Task UploadGameResult(Gamesession session)
        {

            int highscore = (await GetById(session.UserId))!.Highscore;
            if (session.ScoreGathered > highscore)
            {
                highscore = session.ScoreGathered;
            }

            var user = await _userManager.Users.Where(u => u.Id == session.UserId).FirstOrDefaultAsync();

            user.Coins += session.CoinsGathered;
            user.Timespassed += session.PeoplePassed;
            user.Scoregathered += session.ScoreGathered;
            user.Highscore = highscore;

            await _userManager.UpdateAsync(user);
        }


        public  UserDTO CreateDto(IDataRecord reader)
        {
            UserDTO dto = new()
            {
                ID = Convert.ToInt32(reader["IDUser"])
            };
            try
            {
                dto.Avatar = reader["Avatar"].ToString() ?? "";
            }
            catch (Exception e)
            {
                dto.Avatar = "";
            }
           
            dto.Coinsgathered = Convert.ToInt32(reader["Coinsgathered"]);
            dto.Coinsspent = Convert.ToInt16(reader["Coinsspent"]);
            dto.Email = reader["Email"].ToString()!;
            dto.Firstname = reader["Firstname"].ToString()!;
            dto.Lastname = reader["Lastname"].ToString()!;
            dto.Username = reader["Username"].ToString()!;
            dto.Highscore = Convert.ToInt32(reader["Highscore"]);
            dto.IsAdmin = Convert.ToBoolean(reader["IsAdmin"]);
            dto.Itemspurchased = Convert.ToInt32(reader["Itemspurchased"]);
            dto.Coins = Convert.ToInt32(reader["Coins"]);
            return dto;
        }


     
    }
}