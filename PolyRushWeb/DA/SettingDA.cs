using System;
using MySql.Data.MySqlClient;
using PolyRushLibrary;
using PolyRushWeb.Helper;

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
        public int? GetSetting(int id, EnumSetting enumSetting)
        {
            if (!SettingExists(id, enumSetting)) CreateSetting(id, enumSetting);

            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query =
                "SELECT * from usersetting INNER JOIN setting WHERE UserID = @UserID AND SettingID = @SettingID";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserID", id);
            cmd.Parameters.AddWithValue("@SettingID", enumSetting);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            Setting setting = Create(reader);
            reader.Close();
            conn.Close();
            return setting.Idsetting;
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

        private Setting Create(MySqlDataReader reader)
        {
            return new()
            {
                Idsetting = Convert.ToInt32(reader["IDSetting"]),
                Name = reader["Name"].ToString()!,
                Description = reader["Description"].ToString()!
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