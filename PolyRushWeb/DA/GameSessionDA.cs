using System;
using System.Data;
using PolyRushLibrary;
using PolyRushWeb.Helper;
using PolyRushWeb.Models;

namespace PolyRushWeb.DA
{
    public  class GameSessionDA
    {
        private readonly UserDA _userDa;
        private readonly polyrushContext _context;

        public GameSessionDA(UserDA userDa, polyrushContext context)
        {
            _userDa = userDa;
            _context = context;
        }
        public async Task UploadGameSession(Gamesession session)
        {

            await _context.AddAsync(session);
            await _context.SaveChangesAsync();
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