using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using PolyRushLibrary;
using PolyRushWeb.Data;
using PolyRushWeb.Helper;
using PolyRushWeb.Models;

namespace PolyRushWeb.DA
{
    public  class AchievementDA
    {
        private readonly PolyRushWebContext _context;

        public AchievementDA(PolyRushWebContext context)
        {
            
            _context = context;
        }
        public async Task<bool> AddAchievementAsync(int IdUser, int IdAchievement)
        {
            Achievement? achievement = await _context.Achievement.FindAsync(IdAchievement);
            if (achievement == null) throw new NullReferenceException();
            UserAchievement? userAchievement = new() { UserId = IdUser, AchievementId = IdAchievement };
            if (await _context.UserAchievement.AnyAsync(x =>
                    x.UserId == userAchievement.UserId && x.AchievementId == userAchievement.AchievementId))
                return false;
            await _context.UserAchievement.AddAsync(userAchievement);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<Achievement?> GetAchievement(int achievementId)
        {
            return await _context.Achievement.FirstOrDefaultAsync(a => a.Idachievement == achievementId);
        }
        public async Task<List<Achievement>> GetAchievements(int id)
        {
            // get all achievements inner join user achievements, select all achievements with userid
            List<Achievement> achievements = await _context.Achievement
                .Join(_context.UserAchievement, a => a.Idachievement, ua => ua.AchievementId,
                (a, ua) => new { Achievement = a, UserAchievement = ua }).Where(aua => aua.UserAchievement.UserId == id)
                .Select(aua => aua.Achievement)
                .ToListAsync();
            return achievements;
        }
    }
}