using System;
using System.Data;
using PolyRushLibrary;
using PolyRushWeb.Data;
using PolyRushWeb.Helper;
using PolyRushWeb.Models;

namespace PolyRushWeb.DA
{
    public  class GameSessionDA
    {
        private readonly UserDA _userDa;
        private readonly PolyRushWebContext _context;

        public GameSessionDA(UserDA userDa, PolyRushWebContext context)
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
        

    }
}