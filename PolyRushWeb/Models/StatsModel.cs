namespace PolyRushWeb.Models
{
    public class StatsModel
    {
        //tuple, value 1 => average, value 2 => user
        public (int, int) Highscore { get; set; }
        public (int, int) PeoplePassed { get; set; }
        public (int, int) ScoreGathered { get; set; }
        public (int, int) CoinsGathered { get; set; }
        public (int, int) Coins { get; set; }
        public (TimeSpan, TimeSpan) PlayTime { get; set; }

    }
}
