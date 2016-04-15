using System.Collections.Generic;
using System.Linq;
using SteamSharp.steamSpy;
using SteamSharp.steamSpy.models;
using SteamSharp.steamStore;
using SteamSharp.steamStore.models;
using SteamSharp.steamUser;
using SteamSharp.steamUser.models;

namespace SteamSharp
{
    //Main Class for interacting with the library
    //All database code lacks and implementation
    //Some classes/models are not used, this is due to the missing, but planned database features
    // ------------------------------------------------ Naming convention ----------------------------------------------------
    // Functions Prefix:
    //              Get:        Functions that perform a http GET request

    public class SteamSharp
    {
        //Functions that are used in production

        //Get a list of games from the steam store.
        public List<SteamStoreGame> GameListByIds(string[] steamIds)
        {
            var steamStore = new SteamStore();

            return steamIds.Select(steamId => steamStore.GetSteamStoreGameById(steamId)).ToList();
        }

        //public Dictionary<int, SteamStoreGame> DictionaryByIds(string[] steamIds)
        //{
        //    var steamStore = new SteamStore();

        //    return steamIds.Select(steamId => steamStore.GetSteamStoreGameById(steamId)).ToDictionary(game => game.data.steam_appid);
        //}
        //Get a user from the Steam web api 
        public List<SteamUser.Player> SteamUserListByIds(string steamKey, string[] steamUserIds)
        {
            var steamUserWebApi = new SteamApi(steamKey);
            var steamUserList = steamUserWebApi.GetSteamUserData(steamUserIds).players;
            return steamUserList;
        }
        //Get list of the games by user and the time played. Time played for 2 weeks are null for games that user hasn't played within the period.
        //Note: Steam havent always recorded game time. (start 2009)
        //The return is in minutes
        public List<UserGameTime.Game> SteamUserGameTimeListById(string steamKey, string steamUserId)
        {
            var steamUserWebApi = new SteamApi(steamKey);
            var gameTimeList = steamUserWebApi.GetGameTimeForUserById(steamUserId).games;
            return gameTimeList;
        }

        //Steam Spy data about steam games
        //Get Steam spy data for a steam game by its id
        public SteamSpyData GameSteamSpyDataById(string steamId)
        {
            var steamSpyRequest = new SteamSpy();
            var gameData = steamSpyRequest.GetSteamSpyDataById(steamId);

            return gameData;
        }

    }
}
