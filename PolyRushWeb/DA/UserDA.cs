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
        
        private readonly UserManager<User> _userManager;
        private readonly IDbContextFactory<PolyRushWebContext> _contextFactory;

        public UserDA(UserManager<User> userManager, IDbContextFactory<PolyRushWebContext> contextFactory)
        {
            _userManager = userManager;
            _contextFactory = contextFactory;
        }

        public async Task<List<UserDTO>> GetUsers(bool includeAdmin = true)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            var users = await context.Users.FromSqlRaw($"select * from user INNER JOIN userrole ON Id = UserId WHERE RoleId = 1").ToListAsync();
            var adminIds = users.Select(u => u.Id);
            //get all users, if admin is not included, don't query them.
            return includeAdmin ?
                await _userManager.Users.Select(u => u.ToUserDTO()).ToListAsync()! :
                await _userManager.Users.Where(u => !adminIds.Contains(u.Id)).Select(u => u.ToUserDTO()).ToListAsync()!;
        }

        public async Task DeactivateAsync(int id, bool deactivate = true)
        {
            var context = await _contextFactory.CreateDbContextAsync();


            User? user = await _userManager.Users.SingleAsync(u => u.Id == id);
            user.IsActive = !deactivate;
            context.Users.Update(user);
            //context.Entry(user).Property(u => u.IsActive).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task<UserDTO> GetByIdAsync(int userId)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            User? user = await context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            return user.ToUserDTO();
        }
        
        public async Task<bool> HasEnoughCoins(int id, int price)
        {
            return price < await GetCoinsAsync(id);
        }
        
        public async Task<int> GetCoinsAsync(int id)
        {
            //TO DO what??
            var context = await _contextFactory.CreateDbContextAsync();

            DbSet<User>? coins = context.Users;
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
            var context = await _contextFactory.CreateDbContextAsync();
            try
            {
                User user = await context.Users.Where(u => u.Id == model.Id).FirstOrDefaultAsync()!;
                user.Highscore = model.Highscore;
                user.Coins = model.Coins;
                user.Coinsgathered = model.Coinsgathered;
                user.Coinsspent = model.Coinsspent;
                user.Firstname = model.Firstname;
                user.Lastname = model.Lastname;
                user.IsActive = model.IsActive;

                //make sure no relation to the user entity is present
                await context.SaveChangesAsync();

                await _userManager.SetUserNameAsync(new User { Id = model.Id}, model.Username);
                await _userManager.SetEmailAsync(user, model.Email);

                var result = await _userManager.UpdateAsync(user);
                //if updating failed, log the errors in the console.
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        Console.WriteLine(err.Code + " " + err.Description);
                    }
                }
                

                if(model.IsAdmin) await _userManager.AddToRoleAsync(user, "Admin");
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
            var context = await _contextFactory.CreateDbContextAsync();

            var totalPlaytimes = await context.Gamesession.Where(u => u.UserId == userId)
                .Select(u => new { u.StartDateTime, u.EndDateTime }).ToListAsync();
                
            //calculate the difference between start and end, convert to seconds, sum it up, and convert to a timespan
            return TimeSpan.FromSeconds(totalPlaytimes.Select(x => x.EndDateTime.Subtract(x.StartDateTime).TotalSeconds).Sum());
        }
    }
}