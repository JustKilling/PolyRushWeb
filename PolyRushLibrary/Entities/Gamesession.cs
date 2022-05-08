using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        //[NotMapped]
        //public TimeSpan GamesessionPlaytime
        //{
        //    get
        //    {
        //        if (EndDateTime == null || StartDateTime == null) return TimeSpan.Zero;

        //        TimeSpan diff = EndDateTime.Subtract(StartDateTime);
        //        return diff;

        //    }
        //}
    }
}
