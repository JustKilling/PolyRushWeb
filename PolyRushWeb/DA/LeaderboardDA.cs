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

            StatsModel? stats = new StatsModel();

            //highscore
            //get the average highscore
            //get all highscores, if there are no highscores higher then 0, return 0 as the average
            //same for the other stats.
            var highScores = _context.Users.Select(u => u.Highscore).Where(x => x > 0);
            int avgHighscore = await highScores.AnyAsync() ? Convert.ToInt32(await highScores.AverageAsync()) : 0;
            stats.Highscore = (avgHighscore, user.Highscore);

            //coinsgathered
            var coinsgathered = _context.Users.Select(u => u.Coinsgathered).Where(x => x > 0);
            int avgCoinsGathered = await coinsgathered.AnyAsync() ? Convert.ToInt32(await coinsgathered.AverageAsync()) : 0;
            stats.CoinsGathered = (avgCoinsGathered, user.Coinsgathered);

            //coins
            var coins = _context.Users.Select(u => u.Coins).Where(x => x > 0);
            int avgCoins = await coins.AnyAsync() ? Convert.ToInt32(await coins.AverageAsync()) : 0;
            stats.Coins = (avgCoins, user.Coins);

            //People passed
            var peoplepassed = _context.Users.Select(u => u.Timespassed).Where(x => x > 0);
            int avgPeoplePassed = await peoplepassed.AnyAsync() ? Convert.ToInt32(await  peoplepassed.AverageAsync()) : 0;
            stats.PeoplePassed = (avgPeoplePassed, user.Timespassed);

            //Score gathered
            var scoregathered = _context.Users.Select(u => u.Scoregathered).Where(x => x > 0);
            int avgScoreGathered =
                await scoregathered.AnyAsync() ? Convert.ToInt32(await scoregathered.AverageAsync()) : 0;
            stats.ScoreGathered = (avgScoreGathered, user.Scoregathered);

            //PlayTime
            //var gameSessions =
            //    _context.Gamesession.AsEnumerable();
            //var userPlaytimes = gameSessions.Select(gs => new UserPlaytime()
            //{
            //    Playtime = gs.EndDateTime.Subtract(gs.StartDateTime),
            //    User = new UserDTO() { ID = gs.UserId }
            //});
            //List<UserPlaytime> playtimes = new List<UserPlaytime>();
            //foreach (UserPlaytime userPlaytime in userPlaytimes)
            //{
            //    if (playtimes.Any(u => u.User.ID == userPlaytime.User.ID))
            //    {
            //        playtimes = playtimes.Where(u => u.User.ID == userPlaytime.User.ID).Select(x => 
            //            new UserPlaytime(){User = x.User, Playtime = x.Playtime.Add(userPlaytime.Playtime)}).ToList();
            //        continue;
            //    }
            //    playtimes.Add(userPlaytime);
            //}

            //stats.PlayTime = (userPlaytimes.Select(x => x.Playtime.TotalSeconds).Average(x => x), TimeSpan.Zero);
            //double avgPerGameSeconds = test.Select(x => x.Playtime.TotalSeconds).Average();
            //foreach (var t in test)
            //{
            //    Console.WriteLine(t.Playtime.TotalSeconds);
            //}
            //double avgPerGameSecondsForUser = test.Where(up => up.User.ID == id).Select(x => x.Playtime.TotalSeconds).Average();
            //stats.PlayTime = (TimeSpan.FromSeconds(avgPerGameSeconds), TimeSpan.FromSeconds(avgPerGameSecondsForUser) );

            var playtimes = await GetTopPlaytimeAsync(_userManager.Users.Count());
            long avgPlaytimeSeconds =playtimes.Any() ? Convert.ToInt64(playtimes.Select(x => x.Playtime).Average(t => t.TotalSeconds)) : 0;
            TimeSpan avgPlaytime = TimeSpan.FromSeconds(avgPlaytimeSeconds);
            stats.PlayTime = (avgPlaytime, await _userDa.GetUserTotalPlaytimeAsync(user.Id));

            //TODO ?avg per game?
            //TODO COINS SPENT

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
                    Playtime = gs.EndDateTime.Subtract(gs.StartDateTime),
                    User = (_userDa.GetByIdAsync(gs.UserId).GetAwaiter().GetResult())
                })
                .ToListAsync();
            List<IGrouping<int, UserPlaytime>>? uptGrouped = userplaytimesUngrouped.GroupBy(u => u.User.ID).ToList();
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
            List<UserPlaytime>? _ = upts.OrderByDescending(u => u.Playtime.TotalSeconds).Take(amount).ToList();
            return _;


        }

        public async Task<List<NextGoalResponse>> GetNextGoals(int amount, int highscore)
        {
            IList<User>? adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            List<UserDTO>? users = await _context.Users.Where(u => !adminUsers.Contains(u)).Where(u => u.Highscore > 100 && u.Highscore > highscore)
                .OrderBy(u => u.Highscore).Take(amount)
                .Select(u => new UserDTO() { ID = u.Id, Highscore = u.Highscore}).ToListAsync();

            if (users.Count <= 0)
            {
                return new List<NextGoalResponse>
                {
                    new() { UserId = -1, Goal = Convert.ToInt32(Math.Ceiling(highscore * 1.25 / 1000)) * 1000, Rank = 0 }
                };
            }

            List<NextGoalResponse> goalResponses = new();

            foreach(UserDTO? user in users)
            {
                goalResponses.Add(new NextGoalResponse
                {
                    UserId = user.ID,
                    Goal = user.Highscore,
                    Rank = GetPositionByHighscore(user.Highscore)
                }); ;
            }

            return goalResponses;
        }

        private int GetPositionByHighscore(int highscore)
        {
            return _context.Users.Where(u => u.Highscore >= highscore).Select(u => u.Id).Count();
        }

        public async Task UpdateRandomAsync(string username)
        {

            try
            {
                User? user = await _userManager.FindByNameAsync(username);
                user.Coins = rnd.Next(1000, 15000);
                user.Highscore = rnd.Next(7500);
                user.Itemspurchased = rnd.Next(20);
                user.Coinsspent = rnd.Next(user.Itemspurchased * 200);
                user.Coinsgathered = rnd.Next(user.Coins - user.Itemspurchased * 200);
                user.Scoregathered = rnd.Next(user.Highscore *2, user.Highscore*50);
                user.Timespassed = rnd.Next(100);

                await _userManager.UpdateAsync(user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
        }

    }
}