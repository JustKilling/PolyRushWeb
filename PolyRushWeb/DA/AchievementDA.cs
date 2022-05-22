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
        private readonly UserDA _userDa;
        private readonly PolyRushWebContext _context;

        public AchievementDA(PolyRushWebContext context)
        {
            
            _context = context;
        }
        public async Task<bool> AddAchievementAsync(int IdUser, int IdAchievement)
        {
            Achievement? achievement = await _context.Achievement.FindAsync(IdAchievement);
            if (achievement == null) throw new NullReferenceException();
            var userAchievement = new UserAchievement() { UserId = IdUser, AchievementId = IdAchievement };
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
    }
}