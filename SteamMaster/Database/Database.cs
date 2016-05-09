using System.Collections.Generic;
using System.Linq;
using DatabaseCore.lib;
using DatabaseCore.lib.converter.models;
using MongoDB.Driver;
using SteamSharpCore.steamSpy.models;
using SteamSharpCore.steamStore.models;

namespace DatabaseCore
{
    public class Database
    {
        private MongoDb Mongo { get; }

        public Database()
        {
            Mongo = new MongoDb("mongodb://localhost:27017", "SteamSharp", "Games");
        }
        public void InsertGame(SteamStoreGame game, SteamSpyData data)
        {
            Mongo.DbInsertGame(game, data);
        }
        public void InsertGameNoPrice(SteamStoreGame game, SteamSpyData data)
        {
            Mongo.DbInsertGameNoPrice(game, data);
        }

        public Dictionary<int, Game> FindAllGames()
        {
            var filter = Builders<Game>.Filter.Empty;
            var gameList = Mongo.DbFindGameByFilter(filter);
            return gameList.ToDictionary(game => game.SteamAppId);
        }
        public List<Game> FindAllGamesList()
        {
            var filter = Builders<Game>.Filter.Empty;
            var gameList = Mongo.DbFindGameByFilter(filter);
            return gameList;
        }

        public Dictionary<int, Game> FindGamesById(List<int> appIds)
        {
            var gamesList = new List<Game>();
            foreach (var appId in appIds)
            {
                var filter = Builders<Game>.Filter.Eq("SteamAppId", appId);
                gamesList.AddRange(Mongo.DbFindGameByFilter(filter));
            }
            return gamesList.ToDictionary(game => game.SteamAppId);
        }

        public List<Game> FindGameByFilter(FilterDefinition<Game> filter)
        {
            return Mongo.DbFindGameByFilter(filter);
        }

        public void DeleteById(int id)
        {
            var filter = Builders<Game>.Filter.Eq("SteamAppId", id);
            Mongo.DbDeleteByFilter(filter);
        }
    }
}
