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
        //constructor that injects the dependencies
        public UserDA(UserManager<User> userManager, IDbContextFactory<PolyRushWebContext> contextFactory)
        {
            _userManager = userManager;
            _contextFactory = contextFactory;
        }
        //get all users
        public async Task<List<UserDTO>> GetUsers(bool includeAdmin = true)
        {
            //get a db context object
            PolyRushWebContext context = await _contextFactory.CreateDbContextAsync();
            //get all admin ids
            List<User> users = await context.Users.FromSqlRaw($"select * from user INNER JOIN userrole ON Id = UserId WHERE RoleId = 1").ToListAsync();
            //put all ids in a list
            IEnumerable<int> adminIds = users.Select(u => u.Id);
            //get all users, if admin is not included, don't query them.
            return includeAdmin ?
                await _userManager.Users.Select(u => u.ToUserDTO()).AsNoTracking().ToListAsync()! :
                await _userManager.Users.Where(u => !adminIds.Contains(u.Id)).AsNoTracking().Select(u => u.ToUserDTO()).ToListAsync()!;
        }
        //method to deactivate the user, optionally, you can active them
        public async Task DeactivateAsync(int id, bool deactivate = true)
        {

            //get a db context object
            PolyRushWebContext context = await _contextFactory.CreateDbContextAsync();
            //find the user in the db
            User? user = await context.Users.FindAsync(id);
            if (user == null) return;
            //(de)activate the user
            user.IsActive = !deactivate;
           //Save changes
            await context.SaveChangesAsync();
        }
        //method to get user by his id
        public async Task<UserDTO> GetByIdAsync(int userId)
        {
            //get dbcontext
            PolyRushWebContext context = await _contextFactory.CreateDbContextAsync();
            //Get user by id
            User? user = await context.Users.Where(u => u.Id == userId).AsNoTracking().FirstOrDefaultAsync();
            //return user, if null return empty user
            return user == null ? new UserDTO() : user.ToUserDTO();
        }
        
        public async Task<bool> HasEnoughCoins(int id, int price)
        {
            //return if user has enough coins
            return price < await GetCoinsAsync(id);
        }
        
        public async Task<int> GetCoinsAsync(int id)
        {
            PolyRushWebContext context = await _contextFactory.CreateDbContextAsync();
            return await context.Users.Where(u => u.Id == id).Select(u => u .Coins).FirstOrDefaultAsync();
        }
        
        public async Task<bool> RemoveCoins(int id, int coins = -1)
        {
            //check if user has enough coins
            int userCoinAmount = await GetCoinsAsync(id);
            if (userCoinAmount < coins) return false;
            
            //get the user
            User? user = await _userManager.Users.Where(u => u.Id == id).AsNoTracking().FirstOrDefaultAsync();
            //return false if user doesn't exist
            if(user == null) return false;

            //if no coins have been given, remove all coins.
            int amount = coins <= 0 ? userCoinAmount : coins;
            user.Coins -= amount;

            return true;
        }

       
        
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
                    //update the tracked user
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
                   //Save the changes
                    await context.SaveChangesAsync();

                }

                await using (PolyRushWebContext context = await _contextFactory.CreateDbContextAsync())
                {
                    //get the user
                    User? user = await _userManager.Users.Where(u => u.Id == model.Id).FirstOrDefaultAsync();
                    if (user == null) return false;
                    //set its username and email
                    await _userManager.SetUserNameAsync(user, model.Username);
                    await _userManager.SetEmailAsync(user, model.Email);
                    //add admin role if admin alse remove it
                    if (model.IsAdmin) await _userManager.AddToRoleAsync(user, "Admin");
                    else await _userManager.RemoveFromRoleAsync(user, "Admin");
                    //save
                    await context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task UploadGameResult(Gamesession session)
        {
            //get the users highscore
            int highscore = (await GetByIdAsync(session.UserId)).Highscore;
            //update highscore if score gathered is higher
            if (session.ScoreGathered > highscore)
            {
                highscore = session.ScoreGathered;
            }

            User? user = await _userManager.Users.Where(u => u.Id == session.UserId).FirstOrDefaultAsync() ?? new User();
            //set new user props
            user.Coins += session.CoinsGathered;
            user.Timespassed += session.PeoplePassed;
            user.Scoregathered += session.ScoreGathered;
            user.Highscore = highscore;
            user.Coinsgathered += session.CoinsGathered;

            //update the user
            await _userManager.UpdateAsync(user);
        }
        //get a users total playtime
        public async Task<TimeSpan> GetUserTotalPlaytimeAsync(int userId)
        {
            PolyRushWebContext context = await _contextFactory.CreateDbContextAsync();

            var totalPlaytimes = await context.Gamesession.Where(u => u.UserId == userId)
                .Select(u => new { u.StartDateTime, u.EndDateTime }).ToListAsync();
                
            //calculate the difference between start and end, convert to seconds, sum it up, and convert to a timespan
            return TimeSpan.FromSeconds(totalPlaytimes.Select(x => x.EndDateTime.Subtract(x.StartDateTime).TotalSeconds).Sum());
        }
        //check if an email is in use
        public async Task<bool> IsEmailInUse(string email)
        {
            User? user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }  
        //check if an username is in use
        public async Task<bool> IsUsernameInUse(string username)
        {
            User? user = await _userManager.FindByNameAsync(username);
            return user != null;
        }


        //method to disable an account
        public async Task DisableAccountAsync(int id)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            //set user inactive
            var user = await context.Users.FindAsync(id);
            user!.IsActive = false;
            await context.SaveChangesAsync();
            //Set user email to something different so it can be used again if they create
            //a new account with that email
            var user2 = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            await _userManager.SetEmailAsync(user2!, "_" + user.Email);
        }
    }
}