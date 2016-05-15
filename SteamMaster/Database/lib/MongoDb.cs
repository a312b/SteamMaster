using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DatabaseCore.lib.converter.models;
using MongoDB.Driver;
using SteamSharpCore.steamSpy.models;
using SteamSharpCore.steamStore.models;

namespace DatabaseCore.lib
{
    internal class MongoDb
    {
        private MongoClient Client { get; }
        private IMongoDatabase Database { get; }
        private IMongoCollection<Game> Collection { get; } 
        private IMongoCollection<SteamStoreGame> SSGCollection { get; }

        public MongoDb(string connectionString, string database, string collection)
        {
            Client = new MongoClient(connectionString);
            Database = Client.GetDatabase(database);
            Collection = Database.GetCollection<Game>(collection);
            SSGCollection = Database.GetCollection<SteamStoreGame>("SteamStoreCollection");
            TagRankDictionary = LoadTagRank();
        }
        public async void DbInsertGame(SteamStoreGame storeGame, SteamSpyData steamSpy, bool convertToDBGame)
        {
            if (convertToDBGame)
            {
                var document = new Game
                {
                    //Translate the data from the steam store game and the steam spy data. This removes the need for doing this in the UI.
                    Title = storeGame.data.name ?? "",
                    Developer = storeGame.data.developers ?? new List<string>(),
                    Publisher = storeGame.data.publishers ?? new List<string>(),
                    SteamAppId = storeGame.data.steam_appid,
                    Description = Regex.Replace(storeGame.data.detailed_description, "<.*?>", string.Empty),
                    Released = storeGame.data.release_date.coming_soon,
                    //This date varries alot. If the game hasn't been released it will default to the 1st of the month defined by steam.
                    ReleaseDate = storeGame.data.release_date.date ?? "",
                    AveragePlayTime = steamSpy.average_forever,
                    AveragePlayTime2Weeks = steamSpy.average_2weeks,
                    //The price from store is in cents so we convert to EUR here.
                    Price = storeGame.data.price_overview.initial/100.00,
                    AgeRating = storeGame.data.required_age,
                    CoverImage = new Uri(storeGame.data.header_image),
                    StoreLink = new Uri("http://store.steampowered.com/app/" + storeGame.data.steam_appid),
                    Genres = storeGame.data.genres ?? new List<SteamStoreGame.Genre>(),
                    Categories = storeGame.data.categories ?? new List<SteamStoreGame.Category>(),
                    Tags = storeGame.data.tags ?? new List<SteamStoreGame.Tag>(),
                    OwnerCount = steamSpy.owners,
                    Windows = storeGame.data.platforms.windows,
                    Linux = storeGame.data.platforms.linux,
                    Mac = storeGame.data.platforms.mac,
                    MetaCritic = storeGame.data.metacritic?.score ?? 0,
                    DLC = storeGame.data.dlc ?? new List<int>(),
                    RecommenderScore = 0


                };

                foreach (var tag in document.Tags)
                {
                    string tagKey = tag.description.ToLower();
                    tag.TagRank = TagRankDictionary.ContainsKey(tagKey) ? TagRankDictionary[tagKey] : 0;
                }

                await Collection.InsertOneAsync(document);
            }
            else
            {
                await SSGCollection.InsertOneAsync(storeGame);
            }

        }
        


        public async void DbInsertGameNoPrice(SteamStoreGame storeGame, SteamSpyData steamSpy)
        {
            var document = new Game
            {
                //Translate the data from the steam store game and the steam spy data. This removes the need for doing this in the UI.
                Title = storeGame.data.name ?? "",
                Developer = storeGame.data.developers ?? new List<string>(),
                Publisher = storeGame.data.publishers ?? new List<string>(),
                SteamAppId = storeGame.data.steam_appid,
                Description = Regex.Replace(storeGame.data.detailed_description, "<.*?>", string.Empty),
                Released = storeGame.data.release_date.coming_soon,
                //This date varries alot. If the game hasn't been released it will default to the 1st of the month defined by steam.
                ReleaseDate = storeGame.data.release_date.date ?? "",
                AveragePlayTime = steamSpy.average_forever,
                AveragePlayTime2Weeks = steamSpy.average_2weeks,
                //The price from store is in cents so we convert to EUR here.
                Price = 0,
                AgeRating = storeGame.data.required_age,
                CoverImage = new Uri(storeGame.data.header_image),
                StoreLink = new Uri("http://store.steampowered.com/app/" + storeGame.data.steam_appid),
                Genres = storeGame.data.genres ?? new List<SteamStoreGame.Genre>(),
                Categories = storeGame.data.categories ?? new List<SteamStoreGame.Category>(),
                Tags = storeGame.data.tags ?? new List<SteamStoreGame.Tag>(),
                OwnerCount = steamSpy.owners,
                Windows = storeGame.data.platforms.windows,
                Linux = storeGame.data.platforms.linux,
                Mac = storeGame.data.platforms.mac,
                MetaCritic = storeGame.data.metacritic?.score ?? 0,
                DLC = storeGame.data.dlc ?? new List<int>(),
                RecommenderScore = 0

            };

            foreach (var tag in document.Tags)
            {
                string tagKey = tag.description.ToLower();
                tag.TagRank = TagRankDictionary.ContainsKey(tagKey) ? TagRankDictionary[tagKey] : 0;
            }

            await Collection.InsertOneAsync(document);

        }


        public Dictionary<string, double> TagRankDictionary { get; set; }

        public Dictionary<string, double> LoadTagRank()
        {
            Dictionary<string, double> returnDictionary = new Dictionary<string, double>();
            string filePath = @"C:\Shits and giggles\Random Software related stuff\TagsAndRanks.txt";
            StreamReader file = new StreamReader(filePath);

            while (!file.EndOfStream)
            {
                string line = file.ReadLine();
                List<string> lineSplitList = line.Split(':').ToList();
                double rank = 0;
                double.TryParse(lineSplitList[1], out rank);
                returnDictionary.Add(lineSplitList[0].ToLower(), rank);
            }

            return returnDictionary;
        }

        public List<Game> DbFindGameByFilter(FilterDefinition<Game> filter )
        {
            var result = Collection.Find(filter).ToList();
            return result;
        }

        public void DbDeleteByFilter(FilterDefinition<Game> filter )
        {

            Collection.DeleteOne(filter);
        }
       
    }
}
