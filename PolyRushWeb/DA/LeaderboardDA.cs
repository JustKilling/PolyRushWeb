using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using PolyRushLibrary.Responses;

using PolyRushWeb.Helper;
using PolyRushWeb.Models;

namespace PolyRushWeb.DA
{

    public class LeaderboardDA
    {
        private readonly UserDA _userDa;
        private readonly polyrushContext _context;
        private readonly UserManager<User> _userManager;
        private Random rnd = new();

        public LeaderboardDA(UserDA userDa, polyrushContext context, UserManager<User> userManager)
        {
            _userDa = userDa;
            _context = context;
            _userManager = userManager;
        }
        public async Task<List<UserDTO>> GetTopUsers(int amount, bool getImages = true)
        {
            return await _context.Users.Where(u => u.IsAdmin == false).OrderByDescending(u => u.Highscore).Take(amount).Select(u => u.ToUserDTO()).ToListAsync();
        }



        //method to return the top users with playtime
        public async Task<List<UserPlaytime>> GetTopPlaytime(int amount)
        {


            //1
            List<UserPlaytime> userplaytimesUngrouped = await _context.Gamesession
                .Select(gs => new UserPlaytime
                {
                    Playtime = gs.EndDateTime.Subtract(gs.StartDateTime),
                    User = (_userDa.GetById(gs.UserId, true).GetAwaiter().GetResult()).ToUserDTO()
                })
                .ToListAsync();
            List<IGrouping<int, UserPlaytime>>? uptGrouped = userplaytimesUngrouped.GroupBy(u => u.User.ID).ToList();
            List<UserPlaytime> upts = new();
            foreach(var uptGroup in uptGrouped)
            {
                UserPlaytime uptToAdd = new() { Playtime = new TimeSpan(0, 0, 0) };
                foreach(var upt in uptGroup)
                {
                    uptToAdd.User = upt.User;
                    break;
                }
                foreach(var upt2 in uptGroup)
                {
                    uptToAdd.Playtime.Add(upt2.Playtime);
                }
                upts.Add(uptToAdd);
            }
            return upts.OrderBy(u => u.User.ID).ToList();
            //2
            //var userplaytimes = (from gs in _context.Gamesession select new UserPlaytime{ User = new UserDTO { ID = gs.UserId}, Playtime = gs.EndDateTime.Subtract(gs.EndDateTime) });





            //        var teamTotalScores =
            //from player in players
            //group player by player.Team into playerGroup
            //select new
            //{
            //    Team = playerGroup.Key,
            //    TotalScore = playerGroup.Sum(x => x.Score),
            //};

            //MySqlConnection conn = DatabaseConnector.MakeConnection();
            ////user top playtime query
            //string query = "select UserID, sum(timediff(EndDateTime, StartDateTime)) AS 'PlayTime' from gamesession group by UserID Order by PlayTime DESC LIMIT @Limit";
            //MySqlCommand cmd = new(query, conn);
            //cmd.Parameters.AddWithValue("@Limit", amount);
            //MySqlDataReader? reader = cmd.ExecuteReader();
            //List<(UserDTO, int TopPlayTime)> users = new();

            //try
            //{
            //    while (reader.Read())
            //    {
            //        UserDTO user = (await _userDa.GetById(Convert.ToInt32(reader["UserID"]), false))!.ToUserDTO();
            //        users.Add((user, Convert.ToInt32(reader["PlayTime"])));
            //    }

            //    return users;
            //}
            //finally
            //{
            //    reader.Close();

            //    conn.Close();
            //}

        }

        public async Task<List<NextGoalResponse>> GetNextGoals(int amount, int highscore)
        {

            //var usefrs = await _context.Users.FromSqlRaw("select Id, Highscore, Avatar from user").Where(u => u.Highscore > highscore).OrderBy(u => u.Highscore).Take(amount).ToListAsync();
            var users = await _context.Users.Where(u => u.Highscore > 100 && u.Highscore > highscore)
                .OrderBy(u => u.Highscore).Take(amount)
                .Select(u => new UserDTO() { ID = u.Id, Highscore = u.Highscore, Avatar = u.Avatar }).ToListAsync();

            if (users.Count <= 0)
            {
                string avatar = ImageToBase64Helper.ConvertImagePathToBase64String("Media/success.png");
                return new()
                {
                    new() { Avatar = avatar, Goal = Convert.ToInt32(highscore * 1.25f), Rank = 0 }
                };
            }

            List<NextGoalResponse> goalResponses = new();

            for (int i = 0; i < users.Count(); i++)
            {
                var user = users[i];
                goalResponses.Add(new()
                {
                    Avatar = user.Avatar,
                    Goal = user.Highscore,
                    Rank = i + 1
                });
            }

            return goalResponses;
        }

        // private  int GetUserPosition(int id)
        // {
        //     MySqlConnection conn = DatabaseConnector.MakeConnection();
        //     //select the first record of the lowest highscore that is higher then the current highscore.
        //     string query = @"SELECT IDUser, Avatar, Highscore FROM user WHERE Highscore > @Current
        //                     ORDER BY Highscore
        //                     LIMIT 1";
        //     MySqlCommand cmd = new(query, conn);
        //     cmd.Parameters.AddWithValue("@Current", highscore);
        //     
        //     
        //     try
        //     {
        //         return new NextGoalResponse(){Avatar = reader["Avatar"].ToString(), Goal = Convert.ToInt32(reader["Highscore"])};
        //     }
        //     finally
        //     {
        //         conn.Close();
        //     }
        // }

        public async Task UpdateRandomAsync(string username)
        {

            var user = await _userManager.FindByNameAsync(username);
            user.Coins = rnd.Next(1000, 15000);
            user.Highscore = rnd.Next(7500);
            user.Itemspurchased = rnd.Next(20);
            user.Coinsspent = rnd.Next(user.Itemspurchased * 200);
            user.Coinsgathered = rnd.Next(user.Coins - user.Itemspurchased * 200);
            user.Timespassed = rnd.Next(100);

            await _userManager.UpdateAsync(user);
        }


    }
}