using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using SteamSharpCore;
using SteamSharpCore.steamUser.models;
using Filter_System.Filter_Core.Filters;
using Filter_System.Filter_Core.Models;

namespace RecommenderSystemCore.User_Data_Handling.Models
{
    class UserWorkClass // Ikke færdig
    {
        public UserWorkClass(SteamUser.Player user )
        {
            sharpCore = new SteamSharp("DCBF7FBBE0781730FA846CEF21DBE6D5");
            userListGameList = sharpCore.SteamUserGameTimeListById(user.steamid);

            userData = new Tuple<SteamUser.Player, List<UserGameTime.Game>>(user, userListGameList);

        }

        private readonly Database mongoDatabase;

        readonly SteamSharp sharpCore;

        private Tuple<SteamUser.Player, List<UserGameTime.Game>> userData;

        private void LoadGames()
        {
            foreach (var game in userListGameList)
            {
                GameWorkClassList.Add(new UserGameWorkClass(game));
            }

            foreach (var game in GameWorkClassList)
            {
                UserGameIDs.Add(game.appID);
            }
            
            DBGameDictionary = mongoDatabase.FindGamesById(UserGameIDs);
        }
        
        public List<UserGameTime.Game> userListGameList { get; } //GameList directly from user

        public List<UserGameWorkClass> GameWorkClassList { get; } //GameList for work

        public Dictionary<int, Game> DBGameDictionary { get; private set; } //GameList with Games corresponding to the database 

        public List<int>  UserGameIDs { get; }

        public List<UserGameWorkClass> GetUsersMostPlayedGames()
        {
            #region whack kode
            PlayerGameXFilter playTimeForeverFilter = new PlayerGameXFilter(0.5);
            GameValueXFilter ownedFilter = new GameValueXFilter(0.5);
            ownedFilter.OwnerCount();

            Dictionary<int, double> playTimeDictionary = playTimeForeverFilter.Execute(userListGameList);

            List<UserGameWorkClass> returnList = userListGameList.Select(game => new UserGameWorkClass(game, playTimeDictionary[game.appid])).ToList();

            foreach (var game in returnList.Where(game => game.PlayTimeForever < 2))
            {
                returnList.Remove(game);
            }

            returnList.Sort();

            return returnList;
            #endregion


        } 
    }
}
