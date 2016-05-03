using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using SteamSharpCore.steamStore;
using SteamSharpCore.steamStore.models;


namespace RecommenderSystemCore
{
    public class RSCore
    {
        public RSCore(SteamSharpCore.SteamSharp steamOperations)
        {
            SteamOprations = steamOperations;

        }


        public SteamSharpCore.SteamSharp SteamOprations { get; set; }


        Dictionary<int, SteamStoreGame> WorkingDictionary = new Dictionary<int, SteamStoreGame>();


        public void InitiateWorkingDictionary(params string[] GameIDs)
        {
           WorkingDictionary = SteamOprations.GameListByIds(GameIDs).ToDictionary(game => game.data.steam_appid);
        }

        public SteamStoreGame DictionaryGetGameByID(int appID)
        {
            SteamStoreGame returnGame;
            if (WorkingDictionary.TryGetValue(appID, out returnGame))
            {
                throw new ArgumentException($"appID {appID} does not exist");
            }

            return returnGame;
        }

        public List<SteamStoreGame.Tag> DictionaryGetTagByGameID(int appID)
        {
            SteamStoreGame game = DictionaryGetGameByID(appID);
            return game.data.tags;
        }


        public List<SteamStoreGame> SortedRecommendationList()
        {
            throw new NotImplementedException();
        }




    }
}
