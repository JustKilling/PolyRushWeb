using System;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
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
        private readonly polyrushContext _context;

        public SettingDA(polyrushContext context)
        {
            _context = context;
        }

        public async Task<int> GetUserSetting(int id, EnumSetting enumSetting)
        {
            //make the setting record for the user if it doesn't exists
            if (!SettingExists(id, enumSetting)) CreateSetting(id, enumSetting);




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


            return (await _context.Usersetting.Where(us => us.UserId == id && us.SettingId == (int)enumSetting).FirstOrDefaultAsync())!.State;
        }

        private void CreateSetting(int id, EnumSetting setting)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query =
                "INSERT INTO usersetting (UserID, SettingID, State) VALUES (@userid, @settingid, @state)";

            int state = 1;
            if (setting is EnumSetting.MasterVolume) state = 100;

            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@userid", id);
            cmd.Parameters.AddWithValue("@settingid", setting);
            cmd.Parameters.AddWithValue("@state", state);


            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //Check if user has a record with this setting.
        private bool SettingExists(int id, EnumSetting setting)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            string query =
                "SELECT COUNT(IDUserSetting) from usersetting WHERE UserID = @UserID AND SettingID = @SettingID";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserID", id);
            cmd.Parameters.AddWithValue("@SettingID", setting);

            try
            {
                return Convert.ToInt32(cmd.ExecuteScalar()) >= 1;
            }
            finally
            {
                conn.Close();
            }
        }

        private Usersetting Create(MySqlDataReader reader)
        {
            return new()
            {
                UserId = Convert.ToInt32(reader["UserID"]),
                SettingId = Convert.ToInt32(reader["SettingID"]),
                State = Convert.ToInt32(reader["State"])
                //Idsetting = Convert.ToInt32(reader["IDSetting"]),
                //Name = reader["Name"].ToString()!,
                //Description = reader["Description"].ToString()!
            };
        }
      
        public void SetSetting(int id, EnumSetting setting, int state)
        {
            if (!SettingExists(id, setting))
                CreateSetting(id, setting);

            MySqlConnection conn = DatabaseConnector.MakeConnection();
            string query = "UPDATE usersetting SET State = @State WHERE UserID = @UserID AND SettingID = @SettingID";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserID", id);
            cmd.Parameters.AddWithValue("@SettingID", setting);
            cmd.Parameters.AddWithValue("@State", state);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}