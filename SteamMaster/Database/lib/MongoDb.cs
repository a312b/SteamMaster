using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Database.lib.converter.models;
using MongoDB.Bson;
using MongoDB.Driver;
using SteamSharpCore.steamSpy.models;
using SteamSharpCore.steamStore.models;

namespace Database.lib
{
    internal class MongoDb
    {
        private MongoClient Client { get; }
        private IMongoDatabase Database { get; }
        private IMongoCollection<Game> Collection { get; } 

        public MongoDb(string connectionString, string database, string collection)
        {
            Client = new MongoClient(connectionString);
            Database = Client.GetDatabase(database);
            Collection = Database.GetCollection<Game>(collection);
        }
        public async void DbInsertGame(SteamStoreGame storeGame, SteamSpyData steamSpy)
        {
            var document = new Game
            {
                //Translate the data from the steam store game and the steam spy data. This removes the need for doing this in the UI.
                Title = storeGame.data.name,
                Developer = storeGame.data.developers,
                Publisher = storeGame.data.publishers,
                SteamAppId = storeGame.data.steam_appid,
                Description = Regex.Replace(storeGame.data.detailed_description, "<.*?>", string.Empty),
                Released = storeGame.data.release_date.coming_soon,
                //This date varries alot. If the game hasn't been released it will default to the 1st of the month defined by steam.
                ReleaseDate = storeGame.data.release_date.date,
                AveragePlayTime = steamSpy.average_forever,
                AveragePlayTime2Weeks = steamSpy.average_2weeks,
                //The price from store is in cents so we convert to EUR here.
                Price = storeGame.data.price_overview.initial / 100.00,
                AgeRating = storeGame.data.required_age,
                CoverImage = new Uri(storeGame.data.header_image),
                StoreLink = new Uri("http://store.steampowered.com/app/" + storeGame.data.steam_appid),
                Genres = storeGame.data.genres,
                Categories = storeGame.data.categories,
                Tags = storeGame.data.tags,
                OwnerCount = steamSpy.owners,
                Windows = storeGame.data.platforms.windows,
                Linux = storeGame.data.platforms.linux,
                Mac = storeGame.data.platforms.mac

            };

            await Collection.InsertOneAsync(document);

        }
        public async void DbInsertGameNoPrice(SteamStoreGame storeGame, SteamSpyData steamSpy)
        {
            var document = new Game
            {
                //Translate the data from the steam store game and the steam spy data. This removes the need for doing this in the UI.
                Title = storeGame.data.name,
                Developer = storeGame.data.developers,
                Publisher = storeGame.data.publishers,
                SteamAppId = storeGame.data.steam_appid,
                Description = Regex.Replace(storeGame.data.detailed_description, "<.*?>", string.Empty),
                Released = storeGame.data.release_date.coming_soon,
                //This date varries alot. If the game hasn't been released it will default to the 1st of the month defined by steam.
                ReleaseDate = storeGame.data.release_date.date,
                AveragePlayTime = steamSpy.average_forever,
                AveragePlayTime2Weeks = steamSpy.average_2weeks,
                //The price from store is in cents so we convert to EUR here.
                Price = 0,
                AgeRating = storeGame.data.required_age,
                CoverImage = new Uri(storeGame.data.header_image),
                StoreLink = new Uri("http://store.steampowered.com/app/" + storeGame.data.steam_appid),
                Genres = storeGame.data.genres,
                Categories = storeGame.data.categories,
                Tags = storeGame.data.tags,
                OwnerCount = steamSpy.owners,
                Windows = storeGame.data.platforms.windows,
                Linux = storeGame.data.platforms.linux,
                Mac = storeGame.data.platforms.mac

            };

            await Collection.InsertOneAsync(document);

        }

        public List<Game> DbFindGames()
        {
            var filter = Builders<Game>.Filter.Empty;
            return Collection.Find(filter).ToList();
        }

        public List<Game> DbFindGameById(string steamAppId)
        {
            var filter = Builders<Game>.Filter.Eq("SteamAppId", int.Parse(steamAppId));
            var result = Collection.Find(filter).ToList();
            return result;
        }
       
    }
}
