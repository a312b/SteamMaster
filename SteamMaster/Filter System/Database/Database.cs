using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.lib;
using Database.lib.converter.models;
using SteamSharpCore.steamSpy.models;
using SteamSharpCore.steamStore.models;

namespace Database
{
    public class Database
    {
        private MongoDb Mongo { get; }

        public Database()
        {
            Mongo = new MongoDb("mongodb://localhost:27017", "A312B", "Games");
        }
        public void InsertGame(SteamStoreGame game, SteamSpyData data)
        {
            Mongo.DbInsertGame(game, data);
        }
        public void InsertGameNoPrice(SteamStoreGame game, SteamSpyData data)
        {
            Mongo.DbInsertGameNoPrice(game, data);
        }

        public List<Game> FindAllGames()
        {
            return Mongo.DbFindGames();
        }

        public List<Game> FindGameById(string appId)
        {
            return Mongo.DbFindGameById(appId);
        } 
    }
}
