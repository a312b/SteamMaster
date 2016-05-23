using System.Collections.Generic;
using System.Linq;
using SteamSharpCore.steamSpy;
using SteamSharpCore.steamSpy.models;
using SteamSharpCore.steamStore;
using SteamSharpCore.steamStore.models;
using SteamSharpCore.steamUser;
using SteamSharpCore.steamUser.models;

namespace SteamSharpCore
{
    //Main Class for interacting with the library
    //All database code lacks and implementation
    //Some classes/models are not used, this is due to the missing, but planned database features
    // ------------------------------------------------ Naming convention ----------------------------------------------------
    // Functions Prefix:
    //              Get:        Functions that perform a http GET request

    public class SteamSharp
    {
        //Key used for most requests to the server
        //Generate/retrieve one here: http://steamcommunity.com/dev/apikey
        public SteamSharp(string steamKey)
        {
            SteamStore = new SteamStore();
            SteamApi = new SteamApi(steamKey);
            SteamSpy = new SteamSpy();
        }

        private SteamStore SteamStore { get; }
        private SteamApi SteamApi { get; }
        private SteamSpy SteamSpy { get; }
        //Functions that are used in production

        public SteamStoreGame GameById(string steamId)
        {
            return SteamStore.GetSteamStoreGameById(steamId);
        }

        //Get a list of games from the steam store.
        public List<SteamStoreGame> GameListByIds(string[] steamIds)
        {
            return steamIds.Select(steamId => SteamStore.GetSteamStoreGameById(steamId)).ToList();
        }

        //Get a user from the Steam web api 
        public List<SteamUser.Player> SteamUserListByIds(string[] steamUserIds)
        {
            return SteamApi.GetSteamUserData(steamUserIds).players;
        }

        //Get list of the games by user and the time played. Time played for 2 weeks are null for games that user hasn't played within the period.
        //Note: Steam havent always recorded game time. (start 2009)
        //The return is in minutes
        public List<UserGameTime.Game> SteamUserGameTimeListById(string steamUserId)
        {
            return SteamApi.GetGameTimeForUserById(steamUserId).games;
        }

        //Steam Spy data about steam games
        //Get Steam spy data for a steam game by its id
        public SteamSpyData GameSteamSpyDataById(string steamId)
        {
            return SteamSpy.GetSteamSpyDataById(steamId);
        }
    }
}
