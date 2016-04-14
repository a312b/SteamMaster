using System.Collections.Generic;

namespace SteamSharp.steamUser.models
{
    public class UserGameTime
    {
        public class Game
        {
            public int appid { get; set; }
            public int playtime_forever { get; set; }
            public int playtime_2weeks { get; set; }
        }

        
       public int game_count { get; set; }
       public List<Game> games { get; set; }
        

        
    }
}
