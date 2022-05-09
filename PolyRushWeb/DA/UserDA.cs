using System.Data;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            List<UserDTO>? users = new();
            if (getAvatar)
            {
                users = await _userManager.Users.Select(u => u.ToUserDTO()).ToListAsync()!;
            }
         
            return users;
        }

        public async Task<List<UserDTO>> GetUsers()
        {
            List<UserDTO>? users = await _userManager.Users.Select(u => u.ToUserDTO()).ToListAsync()!;
            return users;
        }

        public async Task DeactivateAsync(int id, bool deactivate = true)
        {
            User? user = new()
            {
                Id = id,
                IsActive = !deactivate
            };
            _context.Entry(user).Property(u => u.IsActive).IsModified = true;
            await _context.SaveChangesAsync();
        }

        public  User? UserExists(string usernameoremail)
        {

            _context.Users.AnyAsync(u =>
                u.NormalizedEmail == usernameoremail.ToUpper() || u.NormalizedUserName == usernameoremail.ToUpper());
            return null;
        }

        public async Task<User> GetByIdAsync(int userId, bool getAvatar = true)
        {
            //var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            User user = await _userManager.FindByIdAsync(userId.ToString());
            return user;
        }

        public async Task<string?> GetAvatarById(int userId)
        {
            string? avatar = await _context.Users.Where(u => u.Id == userId).Select(u => u.Avatar).FirstOrDefaultAsync();
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
            DbSet<User>? coins = _context.Users;
            IQueryable<User>? coins2 = coins.Where(u => u.Id == id);
            IQueryable<int>? coins3 = coins2.Select(u => u.Coins);
            int coins4 = await coins3.FirstOrDefaultAsync();
                
            return coins4;
        }
        
        public async Task<bool> RemoveCoins(int id, int coins = -1)
        {
            int userCoinAmount = await GetCoinsAsync(id);
            if (userCoinAmount < coins) return false;
            
            User? user = await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

            //if no coins have been given, remove all coins.
            int amount = coins <= 0 ? userCoinAmount : coins;
            user.Coins -= amount;

            return true;
        }

        public async Task<bool> UpdateUser(UserEditAdminModel model)
        {
            try
            {
                var user = await GetByIdAsync(model.Id);
                user.IsAdmin = model.IsAdmin;
                user.Highscore = model.Highscore;
                user.Coins = model.Coins;
                user.Coinsgathered = model.Coinsgathered;
                user.Coinsspent = model.Coinsspent;
                user.Firstname = model.Firstname;
                user.Lastname = model.Lastname;
                user.SeesAds = model.SeesAds;

                await _userManager.SetUserNameAsync(user, model.Username);
                await _userManager.SetEmailAsync(user, model.Email);

                if(user.IsAdmin) await _userManager.AddToRoleAsync(user, "Admin");
                else await _userManager.RemoveFromRoleAsync(user, "Admin");
                
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task UploadGameResult(Gamesession session)
        {

            int highscore = (await GetByIdAsync(session.UserId))!.Highscore;
            if (session.ScoreGathered > highscore)
            {
                highscore = session.ScoreGathered;
            }

            User? user = await _userManager.Users.Where(u => u.Id == session.UserId).FirstOrDefaultAsync();

            user.Coins += session.CoinsGathered;
            user.Timespassed += session.PeoplePassed;
            user.Scoregathered += session.ScoreGathered;
            user.Highscore = highscore;

            await _userManager.UpdateAsync(user);
        }
     
    }
}