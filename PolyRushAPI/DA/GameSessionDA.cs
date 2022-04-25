using System;
using System.Data;
using MySql.Data.MySqlClient;
using PolyRushAPI.Helper;
using PolyRushLibrary;

namespace PolyRushAPI.DA
{
    public static class GameSessionDA
    {
        public static void UploadGameSession(int id, GameSession session)
        {
            MySqlConnection conn = DatabaseConnector.MakeConnection();

            string query =
                @"INSERT INTO gamesession (UserID, StartDateTime, EndDateTime, ScoreGathered, CoinsGathered, PeoplePassed) 
                VALUES (@UserID, @StartDateTime, @EndDateTime, @ScoreGathered, @CoinsGathered, @PeoplePassed)";

            MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserID", id);
            cmd.Parameters.AddWithValue("@StartDateTime", session.StartDateTime);
            cmd.Parameters.AddWithValue("@EndDateTime", session.EndDateTime);
            cmd.Parameters.AddWithValue("@ScoreGathered", session.ScoreGathered);
            cmd.Parameters.AddWithValue("@CoinsGathered", session.CoinsGathered);
            cmd.Parameters.AddWithValue("@PeoplePassed", session.PeoplePassed);

            cmd.ExecuteNonQuery();
            conn.Close();
            
            //update the user with the results
            UserDA.UploadGameResult(session);
        }
        
        private static GameSession Create(IDataReader reader)
        {
            return new GameSession()
            {
                IDGameSession = Convert.ToInt32(reader["IDGameSession"]),
                UserID = Convert.ToInt32(reader["UserID"]),
                StartDateTime = Convert.ToDateTime(reader["StartDateTime"]),
                EndDateTime = Convert.ToDateTime(reader["EndDateTime"]),
                CoinsGathered = Convert.ToInt32(reader["CoinsGathered"]),
                PeoplePassed = Convert.ToInt32(reader["PeoplePassed"]),
                ScoreGathered = Convert.ToInt32(reader["ScoreGathered"])
            };
        }
    }
}