using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using SteamSharp.steamStore;
using SteamSharp;
using SteamSharp.steamStore.models;


namespace RecommenderSystemCore
{
    public class RSCore
    {
        public RSCore()
        {
                
        }

        public TYPE Type { get; set; }

        Dictionary<int, SteamStoreGame> WorkingDictionary = new Dictionary<int, SteamStoreGame>();

        public void InitiateWorkingDictionary()
        {
            GameListByIDs
        }

    }
}
