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

            Debug.WriteLine(precalculations.Count);

            List<Game> RecommenderList = precalculations.Count > 0
                ? GameRank.GetRankedGameList(precalculations)
                : GameRank.GetRankedGameList();

            RecommenderList = FilterManagement(RecommenderList, User);
            RecommenderList.Sort();

            return RecommenderList;
        }

        private Dictionary<string, double> ReadFromFile()
        {
            Dictionary<string, double> result = new Dictionary<string, double>();

            //File should be in the main folder next to the solution file.
            try
            {
                DirectoryInfo fileFolder = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent;
                string path = fileFolder.FullName + @"\TagsAndRanks.txt";
                StreamReader reader = new StreamReader(path);
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(':');
                    string tag = line[0];
                    double tagGameRank = double.Parse(line[1]);
                
                    result.Add(tag,tagGameRank);
                }
                reader.Close();
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.Message);
            }
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
                "soundtrack", //Not a game
                "sdk", //Software development kit
                "dlc", //Downloadable content
                "demo" //Not a full game - Has the exact same tags as the actual game
            };
            return banList.Any(banWord => currentSegment.Contains(banWord));
        }

        private List<Game> FilterManagement(List<Game> InputList, UserWorkClass User)
        {
            //Controls the weight of the filters
            double MostOwnedValue = 1;
            double AvgPlayedForeverValue = 0;
            double AvgPlayTime2WeeksValue = 0;
            double Metacritic = 1;
            double InputListValue = 0;

            double active = MostOwnedValue + AvgPlayedForeverValue + AvgPlayTime2WeeksValue + Metacritic +
                            InputListValue;


            if (active > 0)
            {
                #region FilterExecution

                InputList.Sort((x, y) => x.RecommenderScore.CompareTo(y.RecommenderScore));

                int score = 1;
                foreach (var game in InputList)
                {
                    game.RecommenderScore = score * InputListValue;
                    score++;
                }

                GameFilterX StandardGameFilter = new GameFilterX();
                PlayerGameFilterX PlayerGameRemoval = new PlayerGameFilterX();


                StandardGameFilter.OwnerCount(MostOwnedValue);
                InputList = StandardGameFilter.Execute(InputList);

                StandardGameFilter.AvgPlayTimeForever(AvgPlayedForeverValue);
                InputList = StandardGameFilter.Execute(InputList);

                StandardGameFilter.AvgPlayTime2Weeks(AvgPlayTime2WeeksValue);
                InputList = StandardGameFilter.Execute(InputList);

                StandardGameFilter.MetaCritic(Metacritic);
                InputList = StandardGameFilter.Execute(InputList);

                InputList = PlayerGameRemoval.Execute(InputList, User.DBGameList);

                InputList.Sort();
                #endregion
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

        //public void writeToFile(List<Game> inputList, string fileName)
        //{
        //    DirectoryInfo fileFolder = new DirectoryInfo(Directory.GetCurrentDirectory());
        //    string path = fileFolder.FullName;
        //    StreamWriter writer = new StreamWriter(path + "\\" + fileName + ".txt", false);

        //    int i = 1;
        //    foreach (var game in inputList)
        //    {
        //        writer.WriteLine($"{i} : {game.Title} : {game.RecommenderScore}");
        //        i++;
        //    }

        //    writer.Close();
        //}

    }
}
