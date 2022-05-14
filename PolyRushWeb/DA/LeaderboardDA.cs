﻿using Microsoft.AspNetCore.Identity;
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
        private readonly Random rnd = new();

        public LeaderboardDA(UserDA userDa, polyrushContext context, UserManager<User> userManager)
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



        //method to return the top users with playtime
        public async Task<List<UserPlaytime>> GetTopPlaytime(int amount)
        {




            //Get user playtimes
            List<UserPlaytime> userplaytimesUngrouped = await _context.Gamesession
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
            var _ = upts.OrderByDescending(u => u.Playtime.TotalSeconds).Take(amount).ToList();
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