using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamSharpCore;
using SteamSharpCore.steamUser.models;

namespace RecommenderSystemCore.User_Data_Handling.Models
{
    class UserWorkClass // Ikke færdig
    {
        public UserWorkClass(SteamUser.Player user )
        {
            sharpCore = new SteamSharp("DCBF7FBBE0781730FA846CEF21DBE6D5");
            userListGameList = sharpCore.SteamUserGameTimeListById(user.steamid);

            userData = new Tuple<SteamUser.Player, List<UserGameTime.Game>>(user, userListGameList);

            foreach (var game in userListGameList)
            {
                GameWorkClassList.Add(new UserGameWorkClass(game));
            }
        }

        readonly SteamSharp sharpCore;

        private Tuple<SteamUser.Player, List<UserGameTime.Game>> userData;
        
        public List<UserGameTime.Game> userListGameList { get; }

        public List<UserGameWorkClass> GameWorkClassList { get; } 
    }
}
