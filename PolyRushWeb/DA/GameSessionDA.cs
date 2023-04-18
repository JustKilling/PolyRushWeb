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
        //constructor that injects the dependencies
        public GameSessionDA(UserDA userDa, PolyRushWebContext context)
        {
            _userDa = userDa;
            _context = context;
        }
        //methat that puts a game session in the db
        public async Task UploadGameSession(Gamesession session)
        {
            //add the session
            await _context.AddAsync(session);
            //add session to table
            await _context.SaveChangesAsync();
            //update the user with the results
            await _userDa.UploadGameResult(session);
        }
        

    }
}