using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using PolyRushLibrary;
using PolyRushLibrary.Responses;
using PolyRushWeb.Helper;
using PolyRushWeb.Models;

namespace PolyRushWeb.DA
{
    public class UserPlaytime
    {
        public UserDTO User { get; set; }
        public TimeSpan Playtime { get; set; }
    }
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
        public async Task<List<(UserDTO, int TopPlayTime)>> GetTopPlaytime(int amount)
        {

            //var gamesessions = await _context.Gamesession.FromSqlInterpolated(
            //    $"select * from gamesession LIMIT {amount}").ToListAsync();

            //var gamesessions = _context.Gamesession.GroupBy(u => u.UserId).Select(gs => new UserPlaytime {Playtime = gs.(x => x.EndDateTime.Subtract(x.StartDateTime)) })
            
            //        var teamTotalScores =
            //from player in players
            //group player by player.Team into playerGroup
            //select new
            //{
            //    Team = playerGroup.Key,
            //    TotalScore = playerGroup.Sum(x => x.Score),
            //};

            MySqlConnection conn = DatabaseConnector.MakeConnection();
            //user top playtime query
            string query = "select UserID, sum(timediff(EndDateTime, StartDateTime)) AS 'PlayTime' from gamesession group by UserID Order by PlayTime DESC LIMIT @Limit";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Limit", amount);
            MySqlDataReader? reader = cmd.ExecuteReader();
            List<(UserDTO, int TopPlayTime)> users = new();

            try
            {
                while (reader.Read())
                {
                    UserDTO user = (await _userDa.GetById(Convert.ToInt32(reader["UserID"]), false))!.ToUserDTO();
                    users.Add((user, Convert.ToInt32(reader["PlayTime"])));
                }

                return users;
            }
            finally
            {
                reader.Close();

                conn.Close();
            }

        }
        
        public async Task<List<NextGoalResponse>> GetNextGoals(int amount, int highscore)
        {

            //var usefrs = await _context.Users.FromSqlRaw("select Id, Highscore, Avatar from user").Where(u => u.Highscore > highscore).OrderBy(u => u.Highscore).Take(amount).ToListAsync();
            var users = await _context.Users.Select(u => new UserDTO() {ID = u.Id,Highscore = u.Highscore, Avatar = u.Avatar }).ToListAsync();

            if(users.Count <= 0)
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
                    Rank = i+1
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
            user.Coins = rnd.Next(1000,15000);
            user.Highscore = rnd.Next(7500);
            user.Itemspurchased = rnd.Next(20);
            user.Coinsspent = rnd.Next(user.Itemspurchased*200);
            user.Coinsgathered = rnd.Next(user.Coins - user.Itemspurchased*200);
            user.Timespassed = rnd.Next(100);

            await _userManager.UpdateAsync(user);
        }     


    }
}