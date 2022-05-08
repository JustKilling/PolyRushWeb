using System;
using Microsoft.EntityFrameworkCore;

using PolyRushLibrary;
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
        private readonly IDbContextFactory<polyrushContext> _contextFactory;

        public SettingDA(IDbContextFactory<polyrushContext> contextFactory)
        {
            _contextFactory = contextFactory;
          
        }

        public async Task<int> GetUserSetting(int id, EnumSetting enumSetting)
        {
            //make the setting record for the user if it doesn't exists
            if (!await SettingExistsAsync(id, enumSetting)) await CreateSettingAsync(id, enumSetting);




            //MySqlConnection conn = DatabaseConnector.MakeConnection();

            //string query =
            //    "SELECT * from usersetting WHERE UserID = @UserID AND SettingID = @SettingID";
            //MySqlCommand cmd = new(query, conn);
            //cmd.Parameters.AddWithValue("@UserID", id);
            //cmd.Parameters.AddWithValue("@SettingID", enumSetting);
            //MySqlDataReader reader = cmd.ExecuteReader();
            //if (!reader.Read()) return null;
            //Usersetting setting = Create(reader);
            //reader.Close();
            //await conn.CloseAsync();

            polyrushContext context = await _contextFactory.CreateDbContextAsync();
            return (await context.Usersetting.Where(us => us.UserId == id && us.SettingId == (int)enumSetting).FirstOrDefaultAsync())!.State;
        }

        private async Task CreateSettingAsync(int id, EnumSetting setting)
        {
            int state = 1;
            if (setting is EnumSetting.MasterVolume) state = 100;
            polyrushContext context = await _contextFactory.CreateDbContextAsync();
            await context.Usersetting.AddAsync(new Usersetting { SettingId = (int)setting, State = state, UserId = id });
        }

        //Check if user has a record with this setting.
        private async Task<bool> SettingExistsAsync(int id, EnumSetting setting)
        {
            bool result = false;
            polyrushContext context = await _contextFactory.CreateDbContextAsync();
            result = context.Usersetting.Where(u => u.UserId == id && u.SettingId == (int)setting).FirstOrDefaultAsync() != null;
            
            return result;
        }
        public async Task SetSettingAsync(int id, EnumSetting setting, int state)
        {
            if (!await (SettingExistsAsync(id, setting)))
                await CreateSettingAsync(id, setting);


            polyrushContext context = await _contextFactory.CreateDbContextAsync();
            
            Usersetting usersetting = await context.Usersetting.SingleAsync(us => us.UserId == id && us.SettingId == (int)setting);
            usersetting.State = state;
            context.Usersetting.Update(usersetting);
            await context.SaveChangesAsync();

            //MySqlConnection conn = DatabaseConnector.MakeConnection();
            //string query = "UPDATE usersetting SET State = @State WHERE UserID = @UserID AND SettingID = @SettingID";
            //MySqlCommand cmd = new(query, conn);
            //cmd.Parameters.AddWithValue("@UserID", id);
            //cmd.Parameters.AddWithValue("@SettingID", setting);
            //cmd.Parameters.AddWithValue("@State", state);
            //cmd.ExecuteNonQuery();
            //conn.Close();
        }

        //private Usersetting Create(MySqlDataReader reader)
        //{
        //    return new()
        //    {
        //        UserId = Convert.ToInt32(reader["UserID"]),
        //        SettingId = Convert.ToInt32(reader["SettingID"]),
        //        State = Convert.ToInt32(reader["State"])
        //        //Idsetting = Convert.ToInt32(reader["IDSetting"]),
        //        //Name = reader["Name"].ToString()!,
        //        //Description = reader["Description"].ToString()!
        //    };
        //}


    }
}