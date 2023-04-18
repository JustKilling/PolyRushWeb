using System;
using Microsoft.EntityFrameworkCore;

using PolyRushLibrary;
using PolyRushWeb.Data;
using PolyRushWeb.Helper;
using PolyRushWeb.Models;


namespace PolyRushWeb.DA
{
    public enum EnumSetting
    {
        MasterVolume = 0,
        Sfx = 1,
        Music = 2
    }

    public class SettingDA
    {
        private readonly IDbContextFactory<PolyRushWebContext> _contextFactory;
        //constructor that injects the dependencies
        public SettingDA(IDbContextFactory<PolyRushWebContext> contextFactory)
        {
            _contextFactory = contextFactory;
          
        }
        //get the value of a certain user setting
        public async Task<int> GetUserSetting(int id, EnumSetting enumSetting)
        {
            //make the setting record for the user if it doesn't exists
            if (!await SettingExistsAsync(id, enumSetting)) await CreateSettingAsync(id, enumSetting);

            PolyRushWebContext context = await _contextFactory.CreateDbContextAsync();
            //return the setting state
            return (await context.Usersetting.Where(us => us.UserId == id && us.SettingId == (int)enumSetting).FirstOrDefaultAsync())!.State;
        }
        //Create a user setting
        private async Task CreateSettingAsync(int id, EnumSetting setting)
        {
            int state = 1;
            //set it to 100 for the master volume
            if (setting is EnumSetting.MasterVolume) state = 100;
            PolyRushWebContext context = await _contextFactory.CreateDbContextAsync();
            //add the setting to the usersetting
            await context.Usersetting.AddAsync(new Usersetting { SettingId = (int)setting, State = state, UserId = id });
            //Save
            await context.SaveChangesAsync();
        }

        //Check if user has a record with this setting.
        private async Task<bool> SettingExistsAsync(int id, EnumSetting setting)
        {
            PolyRushWebContext context = await _contextFactory.CreateDbContextAsync();

            //check if there are any usersettings with that id and setting
            return await context.Usersetting.Where(u => u.UserId == id && u.SettingId == (int)setting).AnyAsync();    
        }
        public async Task SetSettingAsync(int id, EnumSetting setting, int state)
        {
            //make the setting record for the user if it doesn't exists
            if (!await (SettingExistsAsync(id, setting)))
                await CreateSettingAsync(id, setting);


            PolyRushWebContext context = await _contextFactory.CreateDbContextAsync();
            
            //select the usersetting
            Usersetting usersetting = await context.Usersetting.SingleAsync(us => us.UserId == id && us.SettingId == (int)setting);
            usersetting.State = state;
            //Save and update it
            context.Usersetting.Update(usersetting);
            await context.SaveChangesAsync();

        }


    }
}