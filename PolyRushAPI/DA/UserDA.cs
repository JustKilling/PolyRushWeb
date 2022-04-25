using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Mail;
using MySql.Data.MySqlClient;
using PolyRushAPI.Helper;
using PolyRushAPI.Models;
using PolyRushLibrary;

namespace PolyRushAPI.DA
{
    public static class UserDA
    {
        public static List<User> GetUsers()
        {
            try
            {
                MySqlConnection conn = DatabaseConnector.MakeConnection();
                string query = "select * from user";
                MySqlCommand cmd = new(query, conn);
                MySqlDataReader? reader = cmd.ExecuteReader();
                List<User> users = new();
                while (reader.Read()) users.Add(Create(reader));
                return users;
                DatabaseConnector.CloseConnection(conn);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Source + e.Message);
            }

            return new List<User>();
        }


        public static void AddUser(User user)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query =
                "INSERT INTO user (Firstname, Lastname, Username, Email, Password, Salt, Avatar) VALUES (@Firstname, @Lastname, @Username, @Email, @Password, @Salt, @Avatar)";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Firstname", user.Firstname);
            cmd.Parameters.AddWithValue("@Lastname", user.Lastname);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@Salt", user.Salt);

            var avatar = ImageToBase64Helper.ConvertImagePathToBase64String("Media/user.png");
            
            //if no avatar has been provided, use the default image
            if (string.IsNullOrWhiteSpace(user.Avatar)) user.Avatar = avatar;
            user.Avatar = Convert.ToBase64String(ImageToBase64Helper.ReduceImageSize(Convert.FromBase64String(user.Avatar)));
            
            cmd.Parameters.AddWithValue("@Avatar", user.Avatar);

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static bool Login(User u)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            if (UserExists(u.Username) == null) return false;

            string query = "SELECT COUNT(*) FROM USER WHERE Username=@Username AND Password=@Password";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Username", u.Username);
            cmd.Parameters.AddWithValue("@Password", u.Password);

            bool output = Convert.ToInt16(cmd.ExecuteScalar()) >= 1;
            conn.Close();
            return output;
        }
        public static void Logout(int id)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query = "UPDATE user SET RefreshToken = '' WHERE IDUser = @IDUser";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@IDUser", id);
            cmd.ExecuteNonQuery();
            
            conn.Close();
        }
        public static User? UserExists(string usernameoremail)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query =
                $"SELECT * FROM USER WHERE {(IsEmail(usernameoremail) ? "Email" : "Username")}=@usernameoremail";

            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@usernameoremail", usernameoremail);

            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                User u = Create(reader);
                reader.Close();
                conn.Close();
                return u;
            }

            conn.Close();
            return null;
        }

        public static void SetRefreshToken(string usernameoremail, string refreshToken)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query =
                $"UPDATE user SET RefreshToken = @refreshToken WHERE {(IsEmail(usernameoremail) ? "Email" : "Username")} = @usernameoremail";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@refreshToken", refreshToken);
            cmd.Parameters.AddWithValue("@usernameoremail", usernameoremail);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static User? GetById(int userIdUser)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query = "SELECT * from user WHERE IDUser=@IDUser";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@IDUser", userIdUser);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            User user = Create(reader);
            reader.Close();
            conn.Close();
            return user;
        }

        public static bool HasEnoughCoins(int id, int price)
        {
            return price < GetCoins(id);
        }


        public static RefreshToken? GetRefreshToken(RefreshRequest refreshRequestRefreshToken)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query = "SELECT * from user WHERE RefreshToken=@RefreshToken";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@RefreshToken", refreshRequestRefreshToken.RefreshToken);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            RefreshToken rt = new()
            {
                Token = refreshRequestRefreshToken.RefreshToken,
                User = Create(reader)
            };

            reader.Close();
            conn.Close();
            return rt;
        }

        //Een methode om te kijken of dit een email is.
        private static bool IsEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public static int GetCoins(int id)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query = "SELECT Coins from user WHERE IDUser=@IDUser";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@IDUser", id);

            try
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            finally
            {
                conn.Close();
            }
        }
        
        public static bool RemoveCoins(int id, int coins = -1)
        {
            var userCoinAmount = UserDA.GetCoins(id);
            if (userCoinAmount < coins) return false;
            
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query = "UPDATE user SET Coins = Coins - @Coins WHERE IDUser=@IDUser";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@IDUser", id);
            //if no coins have been given, remove all coins.
            var amount = coins == -1 ? userCoinAmount : coins;
            cmd.Parameters.AddWithValue("@Coins", amount);
            cmd.ExecuteNonQuery();
            
            conn.Close();
            return true;
        }
        
        public static void UploadGameResult(GameSession session)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            var highscore = GetById(session.UserID)?.Highscore;
            if (session.ScoreGathered > highscore)
            {
                highscore = session.ScoreGathered;
            }
            
            string query = 
                @"UPDATE user 
                SET Coins = Coins + @Coins, Coinsgathered = Coinsgathered + @Coins, 
                Timespassed = Timespassed + @Timespassed,
                Highscore = @Highscore,
                Scoregathered = Scoregathered + @Score
                WHERE IDUser=@IDUser";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@IDUser", session.UserID);
            cmd.Parameters.AddWithValue("@Coins", session.CoinsGathered);
            cmd.Parameters.AddWithValue("@Timespassed", session.PeoplePassed);
            cmd.Parameters.AddWithValue("@Score", session.ScoreGathered);
            cmd.Parameters.AddWithValue("@Highscore", highscore);
            cmd.ExecuteNonQuery();
        }
        
        private static User Create(IDataRecord reader)
        {
            return new User
            {
                Avatar = reader["Avatar"].ToString(),
                Coinsgathered = Convert.ToInt32(reader["Coinsgathered"]),
                Coinsspent = Convert.ToInt16(reader["Coinsspent"]),
                Email = reader["Email"].ToString(),
                Firstname = reader["Firstname"].ToString(),
                Lastname = reader["Lastname"].ToString(),
                Username = reader["Username"].ToString(),
                Highscore = Convert.ToInt32(reader["Highscore"]),
                IDUser = Convert.ToInt32(reader["IDuser"]),
                IsAdmin = Convert.ToBoolean(reader["IsAdmin"]),
                Itemspurchased = Convert.ToInt32(reader["Itemspurchased"]),
                Password = reader["Password"].ToString(),
                Salt = reader["Salt"].ToString(),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                Coins = Convert.ToInt32(reader["Coins"])
            };
        }
        
        
        public static UserDTO CreateDTO(IDataRecord reader)
        {
            return new UserDTO
            {
                ID = Convert.ToInt32(reader["IDUser"]),
                Avatar = reader["Avatar"].ToString(),
                Coinsgathered = Convert.ToInt32(reader["Coinsgathered"]),
                Coinsspent = Convert.ToInt16(reader["Coinsspent"]),
                Email = reader["Email"].ToString(),
                Firstname = reader["Firstname"].ToString(),
                Lastname = reader["Lastname"].ToString(),
                Username = reader["Username"].ToString(),
                Highscore = Convert.ToInt32(reader["Highscore"]),
                IsAdmin = Convert.ToBoolean(reader["IsAdmin"]),
                Itemspurchased = Convert.ToInt32(reader["Itemspurchased"]),
                Coins = Convert.ToInt32(reader["Coins"])
            };
        }


     
    }
}