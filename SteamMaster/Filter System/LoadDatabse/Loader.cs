using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Database.lib.converter.models;
using SteamSharpCore;
using SteamSharpCore.steamSpy.models;
using SteamSharpCore.steamStore.models;

namespace LoadDatabse
{
    public class Loader
    {
        private SteamSharp SteamSharp { get; }
        private Database.Database Db { get; }

        public Loader()
        {
            SteamSharp = new SteamSharp("");
            Db = new Database.Database();
        }
        public Dictionary<string, string> GetGameIdList()
        {
            var gamesIds = new Dictionary<string, string>();
            int counter = 0;
            string line;

            // Read the file and display it line by line.
            string filePath = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.Parent?.FullName + @"\TagsForAppID.txt";
            StreamReader file = new StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                string[] splitStrings = line.Split(':');
                gamesIds.Add(splitStrings[0], splitStrings[1]);
                counter++;
            }

            file.Close();
            Console.WriteLine($"Total Count: {counter}");
            return gamesIds;
        }

        public void WriteErrorToLog(string key, int count)
        {
            Console.WriteLine($"Error for: {key}, Number: {count}");
            string filePath = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.Parent?.FullName +
                       @"\Errors.csv";
            var fileWriter = new StreamWriter(filePath, true);
            fileWriter.WriteLine($"Error with id: {key}");
            fileWriter.Close();
            
        }

        public async void LoadGamesIntoDatabase()
        {
            int count = 0;
            var list = GetGameIdList();
            foreach (KeyValuePair<string, string> entry in list)
            {
                Console.WriteLine($"Running for: {entry.Key}, Number: {count}");
                SteamStoreGame game = null;
                try
                {
                    game = SteamSharp.GameById(entry.Key);
                }
                catch (ApplicationException)
                {
                    WriteErrorToLog(entry.Key, count);
                }
                if (game?.data != null)
                {
                    SteamSpyData spyData = SteamSharp.GameSteamSpyDataById(entry.Key);
                    string[] tags = entry.Value.Split(',');
                    game.data.tags = new List<SteamStoreGame.Tag>();
                    foreach (var tag in tags)
                    {
                        game.data.tags.Add(new SteamStoreGame.Tag {description = tag});
                    }
                    if (game.data.is_free || game.data.price_overview == null)
                    {
                        Db.InsertGameNoPrice(game, spyData);
                    }
                    else
                    {
                        Db.InsertGame(game, spyData);
                    }
                    count++;
                }
                await Task.Delay(1500);
            }

            Console.WriteLine($"Done, Count is: {count}");
        }

        public void MatchMissing()
        {
            var fullIdList = GetGameIdList();
            var missingIdList = new List<string>();
            var missingList = new Dictionary<string, string>();
            string line;
            string filePath = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.Parent?.FullName + @"\Missing.csv";
            StreamReader file = new StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                string gamesId = line.Trim();
                missingIdList.Add(gamesId);
            }
            file.Close();

        }

        //public void TestDb()
        //{
        //    var gameids = GetGameIdList();
        //    var idList = gameids.Keys.ToList();
        //    Db.CheckDb(idList);
        //}
        //public async void DbCheckGames(List<string> idList)
        //{
        //    foreach (var id in idList)
        //    {
        //        Console.WriteLine($"Checking for id {id}");
        //        var filter = Builders<Game>.Filter.Eq("SteamAppId", int.Parse(id));
        //        var count = await Collection.CountAsync(filter);
        //        if (count > 0)
        //        {
        //            using (var cursor = await Collection.FindAsync(filter))
        //            {
        //                while (await cursor.MoveNextAsync())
        //                {
        //                    var batch = cursor.Current;
        //                    foreach (var document in batch)
        //                    {
        //                        Console.WriteLine($"Game {document.Title} is in the database");

        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Game with {id}, does not exist");
        //            string filePath = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.Parent?.FullName +
        //                       @"\Missing.csv";
        //            var fileWriter = new StreamWriter(filePath, true);
        //            fileWriter.WriteLine(id);
        //            fileWriter.Close();
        //        }

        //    }
        //}

    }
}
