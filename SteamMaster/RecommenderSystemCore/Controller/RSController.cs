using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using Filter_System.Filter_Core.Filters_2._0;
using GameRank;
using RecommenderSystemCore.User_Data_Handling.Models;
using SteamUI;

namespace RecommenderSystemCore.Controller
{
    class RSController
    {
        public RSController(SteamTheme ui)
        {
            database = new Database();
            UI = ui;

            UI.RecommendButtonClick += ExecuteRecommendation;

        }

        private GRGameRank GameRank { get; set; }

        private Database database { get; set; }

        SteamTheme UI { get; set; }

        public void Start()
        {
            UI.Show(); 
        }

        private List<Game> ExecuteRecommendation(string steamID)
        {
            Dictionary<string, double> precalculations = ReadFromFile(); //This should read from the database
            UserWorkClass User = new UserWorkClass(steamID);

            Dictionary<int, Game> dbGames = database.FindAllGames();
            dbGames = RemoveGamesWithBlacklistedWords(dbGames);
            dbGames = RemoveDLCFromList(dbGames);
            GameRank = new GRGameRank(dbGames, User.userListGameList);

            List<Game> RecommenderList = GameRank.GetRankedGameList(precalculations);
            RecommenderList = FilterManagement(RecommenderList, User);
            RecommenderList.Sort();

            return RecommenderList;
        }

        private Dictionary<string, double> ReadFromFile()
        {
            //File can be found in the main GameRank folder. Should be placed in your My Documents folder
            DirectoryInfo fileFolder = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent;
            string path = fileFolder.FullName + @"\TagsAndRanks.txt";
            StreamReader reader = new StreamReader(path);
            Dictionary<string, double> result = new Dictionary<string, double>();
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(':');
                string tag = line[0];
                double tagGameRank = double.Parse(line[1]);
                
                result.Add(tag,tagGameRank);
            }
            reader.Close();
            return result;

        }

        private Dictionary<int, Game> RemoveGamesWithBlacklistedWords(Dictionary<int, Game> dbGames)
        {
            List<int> bannedGames = new List<int>();
            //The linq though
            foreach (Game game in dbGames.Values)
            {
                string[] segments = game.Title.Split(' ');
                if (segments.Any(CheckSegment))
                {
                    bannedGames.Add(game.SteamAppId);
                }
            }
            foreach (int id in bannedGames)
            {
                dbGames.Remove(id);
            }
            return dbGames;
        }

        private bool CheckSegment(string segment)
        {
            string currentSegment = segment.Where(char.IsLetter).Aggregate("", (current, ch) => current + ch).ToLower();
            List<string> banList = new List<string>()
            {
                "soundtrack",
                "sdk",
                "dlc",
                "demo"
            };
            return banList.Any(banWord => currentSegment.Contains(banWord));
        }

        private List<Game> FilterManagement(List<Game> InputList, UserWorkClass User)
        {
            List<Game> FilterWorkList = InputList;
            foreach (var game in FilterWorkList)
            {
                game.RecommenderScore = 0;
            }
            //Controls the weight of the filters
            double MostOwnedValue = 0.3;
            double AvgPlayedForeverValue = 0;
            double AvgPlayTime2WeeksValue = 0;
            double Metacritic = 0.1;

            GameFilterX StandardGameFilter = new GameFilterX();
            PlayerGameFilterX PlayerGameRemoval = new PlayerGameFilterX();
            

            StandardGameFilter.OwnerCount(MostOwnedValue);
            FilterWorkList = StandardGameFilter.Execute(FilterWorkList);

            StandardGameFilter.AvgPlayTimeForever(AvgPlayedForeverValue);
            FilterWorkList = StandardGameFilter.Execute(FilterWorkList);

            StandardGameFilter.AvgPlayTime2Weeks(AvgPlayTime2WeeksValue);
            FilterWorkList = StandardGameFilter.Execute(FilterWorkList);

            StandardGameFilter.MetaCritic(Metacritic);
            FilterWorkList = StandardGameFilter.Execute(FilterWorkList);

            FilterWorkList = PlayerGameRemoval.Execute(FilterWorkList, User.DBGameList);

            Dictionary<int, double> FilterWorkDictionary = FilterWorkList.ToDictionary(game => game.SteamAppId,
                game => game.RecommenderScore);

            foreach (var game in InputList)
            {
                int appID = game.SteamAppId;
                if (FilterWorkDictionary.ContainsKey(appID))
                {
                    game.RecommenderScore *= FilterWorkDictionary[appID];
                }
            }

            return InputList;
        }

        public Dictionary<int, Game> RemoveDLCFromList(Dictionary<int, Game> gameDictionary)
        {
            List<int> gameDLCList = gameDictionary.SelectMany(game => game.Value.DLC).ToList();

            foreach (var appID in gameDLCList)
            {
                if (gameDictionary.ContainsKey(appID))
                {
                    gameDictionary.Remove(appID);
                }
            }

            return gameDictionary;
        }
    }
}
