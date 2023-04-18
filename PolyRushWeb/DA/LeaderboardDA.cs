using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using PolyRushLibrary.Responses;
using PolyRushWeb.Data;
using PolyRushWeb.Helper;
using PolyRushWeb.Models;

namespace PolyRushWeb.DA
{

    public class LeaderboardDA
    {
        private readonly UserDA _userDa;
        private readonly PolyRushWebContext _context;
        private readonly UserManager<User> _userManager;
        private readonly Random rnd = new();

        public LeaderboardDA(UserDA userDa, PolyRushWebContext context, UserManager<User> userManager)
        {
            _userDa = userDa;
            _context = context;
            _userManager = userManager;
        }
        public async Task<List<UserDTO>> GetTopUsers(int amount)
        {
            IList<User>? adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            return await _userManager.Users.Where(u => !adminUsers.Contains(u)).OrderByDescending(u => u.Highscore).Take(amount).Select(u => u.ToUserDTO()).ToListAsync();
        }

        public async Task<StatsModel> GetUserStats(int id)
        {
            //get first user with that id.
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return new StatsModel();

            StatsModel? stats = new();

            //highscore
            //get the average highscore
            //get all highscores, if there are no highscores higher then 0, return 0 as the average
            //same for the other stats.
            IQueryable<int>? highScores = _context.Users.Select(u => u.Highscore).Where(x => x > 0);
            int avgHighscore = await highScores.AnyAsync() ? Convert.ToInt32(await highScores.AverageAsync()) : 0;
            stats.Highscore = (avgHighscore, user.Highscore);

            //coinsgathered
            //select how much coins each user has gathered, calculate the average and put it in the stats object
            IQueryable<int>? coinsgathered = _context.Users.Select(u => u.Coinsgathered).Where(x => x > 0);
            int avgCoinsGathered = await coinsgathered.AnyAsync() ? Convert.ToInt32(await coinsgathered.AverageAsync()) : 0;
            stats.CoinsGathered = (avgCoinsGathered, user.Coinsgathered);

            //coins
            //select how much coins each user has
            IQueryable<int>? coins = _context.Users.Select(u => u.Coins).Where(x => x > 0);
            //calculate the average
            int avgCoins = await coins.AnyAsync() ? Convert.ToInt32(await coins.AverageAsync()) : 0;
            stats.Coins = (avgCoins, user.Coins);

            //People passed
            IQueryable<int>? peoplepassed = _context.Users.Select(u => u.Timespassed).Where(x => x > 0);
            int avgPeoplePassed = await peoplepassed.AnyAsync() ? Convert.ToInt32(await  peoplepassed.AverageAsync()) : 0;
            stats.PeoplePassed = (avgPeoplePassed, user.Timespassed);

            //Score gathered
            //select all scores that are gathered from each user
            IQueryable<int>? scoregathered = _context.Users.Select(u => u.Scoregathered).Where(x => x > 0);
            //calculate the average
            int avgScoreGathered =
                await scoregathered.AnyAsync() ? Convert.ToInt32(await scoregathered.AverageAsync()) : 0;
            //put it in the stats
            stats.ScoreGathered = (avgScoreGathered, user.Scoregathered);

            //PlayTime
            //get the top playtimes for all users
            List<UserPlaytime>? playtimes = await GetTopPlaytimeAsync(_userManager.Users.Count());
            //calculate the average playtime in seconds
            long avgPlaytimeSeconds =playtimes.Any() ? Convert.ToInt64(playtimes.Select(x => x.Playtime).Average(t => t.TotalSeconds)) : 0;
            //put it in a timespan object and put it in the playtime object
            TimeSpan avgPlaytime = TimeSpan.FromSeconds(avgPlaytimeSeconds);
            stats.PlayTime = (avgPlaytime, await _userDa.GetUserTotalPlaytimeAsync(user.Id));

            return stats;
        }

        //method to return the top users with playtime
        public async Task<List<UserPlaytime>> GetTopPlaytimeAsync(int amount)
        {
            
            //select all non admins
            List<int> nonAdminIds = (await _userDa.GetUsers(false)).Select(u => u.ID).ToList();
            //Get user playtimes
            List<UserPlaytime> userplaytimesUngrouped = await _context.Gamesession.Where(gs => nonAdminIds.Contains(gs.UserId))
                .Select(gs => new UserPlaytime
                {
                    //caclulate how long the gamesession took
                    Playtime = gs.EndDateTime.Subtract(gs.StartDateTime),
                    //get the user
                    User = (_userDa.GetByIdAsync(gs.UserId).GetAwaiter().GetResult())
                })
                .ToListAsync();
            //group the object by the id
            List<IGrouping<int, UserPlaytime>>? uptGrouped = userplaytimesUngrouped.GroupBy(u => u.User.ID).ToList();
            //remove reduntant records by adding the playtimes together
            List<UserPlaytime> upts = new();
            foreach (IGrouping<int, UserPlaytime>? uptGroup in uptGrouped)
            {
                UserPlaytime uptToAdd = new() { Playtime = new TimeSpan(0, 0, 0) };
                foreach (UserPlaytime? upt in uptGroup)
                {
                    uptToAdd.User = upt.User;
                    break;
                }
                foreach (UserPlaytime? upt2 in uptGroup)
                {
                    uptToAdd.Playtime = uptToAdd.Playtime.Add(upt2.Playtime);
                }
                upts.Add(uptToAdd);
            }
            //order the list by descending order so higher user is on top and take the certain amount
            return upts.OrderByDescending(u => u.Playtime.TotalSeconds).Take(amount).ToList();
        }
        //get the next goal
        public async Task<List<NextGoalResponse>> GetNextGoals(int amount, int highscore)
        {
            //select all admins
            IList<User>? adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            //select all non admins where highscore is higher then 100 and higher then the users highscore, order by the highscore and take a certain amount
            //then select the user object
            List<UserDTO>? users = await _context.Users.Where(u => !adminUsers.Contains(u)).Where(u => u.Highscore > 100 && u.Highscore > highscore)
                .OrderBy(u => u.Highscore).Take(amount)
                .Select(u => new UserDTO() { ID = u.Id, Highscore = u.Highscore}).ToListAsync();

            //if it doesn't have any users it means the user is on top so return their highscore * 1.25
            if (users.Count <= 0)
            {
                return new List<NextGoalResponse>
                {
                    new() { UserId = -1, Goal = Convert.ToInt32(Math.Ceiling(highscore * 1.25 / 1000)) * 1000, Rank = 0 }
                };
            }
            //add the goalsresponses to the object
            List<NextGoalResponse> goalResponses = new();
            foreach(UserDTO? user in users)
            {
                goalResponses.Add(new NextGoalResponse
                {
                    UserId = user.ID,
                    Goal = user.Highscore,
                    //get the user positoin
                    Rank = GetPositionByHighscore(user.Highscore)
                }); ;
            }
            //return the goal responses object

            return goalResponses;
        }
        //select what position the user is by counting how much users are above you
        private int GetPositionByHighscore(int highscore)
        {
            return _context.Users.Where(u => u.Highscore >= highscore).Select(u => u.Id).Count();
        }

        //public async Task UpdateRandomAsync(string username)
        //{
        //    try
        //    {
        //        User? user = await _userManager.FindByNameAsync(username);
        //        user.Coins = rnd.Next(1000, 15000);
        //        user.Highscore = rnd.Next(7500);
        //        user.Itemspurchased = rnd.Next(20);
        //        user.Coinsspent = rnd.Next(user.Itemspurchased * 200);
        //        user.Coinsgathered = rnd.Next(user.Coins - user.Itemspurchased * 200);
        //        user.Scoregathered = rnd.Next(user.Highscore *2, user.Highscore*50);
        //        user.Timespassed = rnd.Next(100);

        //        await _userManager.UpdateAsync(user);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}
    }
}