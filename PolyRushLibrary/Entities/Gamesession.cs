using System.ComponentModel.DataAnnotations;

namespace PolyRushLibrary
{
    public partial class Gamesession
    {
        [Key]
        public int IdgameSession ;
        public int UserId ;
        public DateTime StartDateTime ;
        public DateTime EndDateTime ;
        public int ScoreGathered ;
        public int CoinsGathered ;
        public int PeoplePassed ;
    }
}
