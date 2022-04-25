using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Helper;
using MySql.Data.MySqlClient;
using PolyRushAPI.Helper;
using PolyRushLibrary;
using PolyRushLibrary.Responses;

namespace PolyRushAPI.DA
{
    public static class LeaderboardDA
    {
        public static List<UserDTO> GetTopUsers(int amount)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            string query = "SELECT * from user WHERE IsAdmin NOT LIKE 1 ORDER BY Highscore DESC LIMIT @Limit";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Limit", amount); 
            MySqlDataReader? reader = cmd.ExecuteReader();
            List<UserDTO> users = new();

            try
            {
                while (reader.Read()) users.Add(UserDA.CreateDTO(reader));

                return users;
            }
            finally
            {
                reader.Close();

                conn.Close();
            }
        }
        public static List<NextGoalResponse> GetNextGoals(int amount, int highscore)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            //select the first record of the lowest highscore that is higher then the current highscore.
            string query = @"SELECT (SELECT 
                            COUNT(*) AS Rank 
                            FROM user 
                            WHERE Highscore>= @Current) as rank, Highscore, Avatar FROM user WHERE Highscore > @Current
                            ORDER BY Highscore
                            LIMIT @Amount";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Current", highscore);
            cmd.Parameters.AddWithValue("@Amount", amount);

            MySqlDataReader reader = cmd.ExecuteReader();
            
            if (!reader.Read())
            {
                conn.Close(); 
                reader.Close();

                var avatar = ImageToBase64Helper.ConvertImagePathToBase64String("Media/success.png");
                return new List<NextGoalResponse>()
                {
                    new NextGoalResponse {Avatar = avatar, Goal = Convert.ToInt32(highscore * 1.25f), Rank = 0}
                };
            }

            try
            {
                List<NextGoalResponse> goalResponses = new List<NextGoalResponse>();
                do
                {
                    goalResponses.Add(new NextGoalResponse()
                    {
                        Avatar = reader["Avatar"].ToString(), Goal = Convert.ToInt32(reader["Highscore"]),
                        Rank = Convert.ToInt32(reader["Rank"])
                    });
                } while (reader.Read());

                return goalResponses;
            }
            finally
            {
                reader.Close();
                conn.Close();
            }
        }

        // private static int GetUserPosition(int id)
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

        public static void UpdateRandom(string username)
        {
            Random rnd = new();
            MySqlConnection conn = DatabaseConnector.MakeConnection();
            string query =
                "Update user SET  Highscore = @Highscore, Coins = @Coins, Itemspurchased=@Itemspurchased, Coinsspent = @Coinsspent, Coinsgathered = @Coinsgathered, Timespassed = @Timespassed where Username = @username";
            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Highscore", rnd.Next(10000));
            cmd.Parameters.AddWithValue("@Itemspurchased", rnd.Next(100));
            cmd.Parameters.AddWithValue("@Coinsspent", rnd.Next(10000));
            cmd.Parameters.AddWithValue("@Coinsgathered", rnd.Next(10000));
            cmd.Parameters.AddWithValue("@Coins", rnd.Next(100000));
            cmd.Parameters.AddWithValue("@Timespassed", rnd.Next(1000));
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.ExecuteNonQuery();
            conn.Close();
        }


    }
}