namespace SteamSharp.steamSpy.models
{
    public class SteamSpyData
    {
        public int appid { get; set; }
        public string name { get; set; }
        public string developer { get; set; }
        public string publisher { get; set; }
        public int score_rank { get; set; }
        public int owners { get; set; }
        public int owners_variance { get; set; }
        public int players_forever { get; set; }
        public int players_forever_variance { get; set; }
        public int players_2weeks { get; set; }
        public int players_2weeks_variance { get; set; }
        public int average_forever { get; set; }
        public int average_2weeks { get; set; }
        public int median_forever { get; set; }
        public int median_2weeks { get; set; }
        public int ccu { get; set; }
    }
}
