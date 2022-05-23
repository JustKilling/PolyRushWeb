using System.Data;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PolyRushWeb.Data;
using PolyRushWeb.Helper;
using PolyRushWeb.Models;

namespace PolyRushWeb.DA
{
    public class UserDA
    {
        private readonly PolyRushWebContext _context;
        private readonly UserManager<User> _userManager;

        public UserDA(PolyRushWebContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<UserDTO>> GetUsers(bool includeAdmin = true)
        {
            var users = await _context.Users.FromSqlRaw($"select * from user INNER JOIN userrole ON Id = UserId WHERE RoleId = 1").ToListAsync();
            var adminIds = users.Select(u => u.Id);
            //get all users, if admin is not included, don't query them.
            return includeAdmin ?
                await _userManager.Users.Select(u => u.ToUserDTO()).ToListAsync()! :
                await _userManager.Users.Where(u => !adminIds.Contains(u.Id)).Select(u => u.ToUserDTO()).ToListAsync()!;
        }

        public async Task DeactivateAsync(int id, bool deactivate = true)
        {

            User? user = await _userManager.Users.SingleAsync(u => u.Id == id);
            user.IsActive = !deactivate;
            _context.Users.Update(user);
            //_context.Entry(user).Property(u => u.IsActive).IsModified = true;
            await _context.SaveChangesAsync();
        }

        public async Task<UserDTO> GetByIdAsync(int userId)
        {
            User? user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            return user.ToUserDTO();
        }
        
        public async Task<bool> HasEnoughCoins(int id, int price)
        {
            return price < await GetCoinsAsync(id);
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
                User user = (await GetByIdAsync(model.Id)).ToUser();
                user.IsAdmin = model.IsAdmin;
                user.Highscore = model.Highscore;
                user.Coins = model.Coins;
                user.Coinsgathered = model.Coinsgathered;
                user.Coinsspent = model.Coinsspent;
                user.Firstname = model.Firstname;
                user.Lastname = model.Lastname;
                user.SeesAds = model.SeesAds;
                user.IsActive = model.IsActive;

                await _userManager.SetUserNameAsync(user, model.Username);
                await _userManager.SetEmailAsync(user, model.Email);

                if(user.IsAdmin) await _userManager.AddToRoleAsync(user, "Admin");
                else await _userManager.RemoveFromRoleAsync(user, "Admin");
                
                return true;
            }
            catch (Exception)
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
            user.Coinsgathered += session.CoinsGathered;

            await _userManager.UpdateAsync(user);
        }

        public async Task<TimeSpan> GetUserTotalPlaytimeAsync(int userId)
        {
            var totalPlaytimes = await _context.Gamesession.Where(u => u.UserId == userId)
                .Select(u => new { u.StartDateTime, u.EndDateTime }).ToListAsync();
                
            //calculate the difference between start and end, convert to seconds, sum it up, and convert to a timespan
            return TimeSpan.FromSeconds(totalPlaytimes.Select(x => x.EndDateTime.Subtract(x.StartDateTime).TotalSeconds).Sum());
        }
    }
}