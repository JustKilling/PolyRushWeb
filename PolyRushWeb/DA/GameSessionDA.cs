using System;
using System.Data;
using MySql.Data.MySqlClient;
using PolyRushLibrary;
using PolyRushWeb.Helper;

namespace PolyRushWeb.DA
{
    public  class GameSessionDA
    {
        private readonly UserDA _userDa;

        public GameSessionDA(UserDA userDa)
        {
            _userDa = userDa;
        }
        public async Task UploadGameSession(int id, Gamesession session)
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
            await _userDa.UploadGameResult(session);
        }
        
        private static Gamesession Create(IDataReader reader)
        {
            return new()
            {
                IdgameSession = Convert.ToInt32(reader["IDGameSession"]),
                UserId = Convert.ToInt32(reader["UserID"]),
                StartDateTime = Convert.ToDateTime(reader["StartDateTime"]),
                EndDateTime = Convert.ToDateTime(reader["EndDateTime"]),
                CoinsGathered = Convert.ToInt32(reader["CoinsGathered"]),
                PeoplePassed = Convert.ToInt32(reader["PeoplePassed"]),
                ScoreGathered = Convert.ToInt32(reader["ScoreGathered"])
            };
        }
    }
}