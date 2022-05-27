using System.Data;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
            PolyRushWebContext context = await _contextFactory.CreateDbContextAsync();

            List<User> users = await context.Users.FromSqlRaw($"select * from user INNER JOIN userrole ON Id = UserId WHERE RoleId = 1").ToListAsync();
            IEnumerable<int> adminIds = users.Select(u => u.Id);
            //get all users, if admin is not included, don't query them.
            return includeAdmin ?
                await _userManager.Users.Select(u => u.ToUserDTO()).AsNoTracking().ToListAsync()! :
                await _userManager.Users.Where(u => !adminIds.Contains(u.Id)).AsNoTracking().Select(u => u.ToUserDTO()).ToListAsync()!;
        }

        public async Task DeactivateAsync(int id, bool deactivate = true)
        {
            PolyRushWebContext context = await _contextFactory.CreateDbContextAsync();


            User? user = await context.Users.FindAsync(id);
            if (user == null) return;
            user.IsActive = !deactivate;
           
            await context.SaveChangesAsync();
        }

        public async Task<UserDTO> GetByIdAsync(int userId)
        {
            PolyRushWebContext context = await _contextFactory.CreateDbContextAsync();

            User? user = await context.Users.Where(u => u.Id == userId).AsNoTracking().FirstOrDefaultAsync();
            return user == null ? new UserDTO() : user.ToUserDTO();
        }
        
        public async Task<bool> HasEnoughCoins(int id, int price)
        {
            return price < await GetCoinsAsync(id);
        }
        
        public async Task<int> GetCoinsAsync(int id)
        {
            //TO DO what??
            PolyRushWebContext context = await _contextFactory.CreateDbContextAsync();

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
            
            User? user = await _userManager.Users.Where(u => u.Id == id).AsNoTracking().FirstOrDefaultAsync();

            if(user == null) return false;

            //if no coins have been given, remove all coins.
            int amount = coins <= 0 ? userCoinAmount : coins;
            user.Coins -= amount;

            return true;
        }

        #region Don't ask why this code works like this, there was alot of trial and error.
        
        //method to update a user
        public async Task<bool> UpdateUser(UserDTO model)
        {
            
            try
            {
                await using (PolyRushWebContext context = await _contextFactory.CreateDbContextAsync())
                {
                    User user = await context.Users.FindAsync(model.ID) ?? new User();
                    user.Firstname = model.Firstname;
                    user.Lastname = model.Lastname;
                    
                    await context.SaveChangesAsync();

                }
                await using (PolyRushWebContext context = await _contextFactory.CreateDbContextAsync())
                {
                    User user = await _userManager.Users.Where(u => u.Id == model.ID).FirstOrDefaultAsync() ?? new User();
                    await _userManager.SetUserNameAsync(user, model.Username);
                    await _userManager.SetEmailAsync(user, model.Email);
                    await context.SaveChangesAsync();
                }
               
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //method to update a user, performed by an admin
        public async Task<bool> UpdateUser(UserEditAdminModel model)
        {
            try
            {
                await using (PolyRushWebContext context = await _contextFactory.CreateDbContextAsync())
                {
                    User? user = await context.Users.FindAsync(model.Id);
                    if (user == null) return false;
                    user.Firstname = model.Firstname;
                    user.Lastname = model.Lastname;
                    user.Highscore = model.Highscore;
                    user.Coins = model.Coins;
                    user.Coinsgathered = model.Coinsgathered;
                    user.Coinsspent = model.Coinsspent;
                    user.IsActive = model.IsActive;
                    user.Scoregathered = model.Scoregathered;
                    user.Itemspurchased = model.Itemspurchased;
                    user.Timespassed = model.Timespassed;
                   
                    await context.SaveChangesAsync();

                }

                await using (PolyRushWebContext context = await _contextFactory.CreateDbContextAsync())
                {
                    User? user = await _userManager.Users.Where(u => u.Id == model.Id).FirstOrDefaultAsync();
                    if (user == null) return false;
                    await _userManager.SetUserNameAsync(user, model.Username);
                    await _userManager.SetEmailAsync(user, model.Email);
                    if (model.IsAdmin) await _userManager.AddToRoleAsync(user, "Admin");
                    else await _userManager.RemoveFromRoleAsync(user, "Admin");
                    await context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        public async Task UploadGameResult(Gamesession session)
        {

            int highscore = (await GetByIdAsync(session.UserId)).Highscore;
            if (session.ScoreGathered > highscore)
            {
                highscore = session.ScoreGathered;
            }

            User? user = await _userManager.Users.Where(u => u.Id == session.UserId).FirstOrDefaultAsync() ?? new User();

            user.Coins += session.CoinsGathered;
            user.Timespassed += session.PeoplePassed;
            user.Scoregathered += session.ScoreGathered;
            user.Highscore = highscore;
            user.Coinsgathered += session.CoinsGathered;

            await _userManager.UpdateAsync(user);
        }

        public async Task<TimeSpan> GetUserTotalPlaytimeAsync(int userId)
        {
            PolyRushWebContext context = await _contextFactory.CreateDbContextAsync();

            var totalPlaytimes = await context.Gamesession.Where(u => u.UserId == userId)
                .Select(u => new { u.StartDateTime, u.EndDateTime }).ToListAsync();
                
            //calculate the difference between start and end, convert to seconds, sum it up, and convert to a timespan
            return TimeSpan.FromSeconds(totalPlaytimes.Select(x => x.EndDateTime.Subtract(x.StartDateTime).TotalSeconds).Sum());
        }

        public async Task<bool> IsEmailInUse(string email)
        {
            User? user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }  
        public async Task<bool> IsUsernameInUse(string username)
        {
            User? user = await _userManager.FindByNameAsync(username);
            return user != null;
        }

      
    }
}