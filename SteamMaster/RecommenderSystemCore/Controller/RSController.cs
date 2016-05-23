using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using Filter_System;
using GameRank;
using SteamSharpCore;
using SteamSharpCore.steamUser.models;
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
            Filter = new FilterControl();
            sharpCore = new SteamSharp("DCBF7FBBE0781730FA846CEF21DBE6D5");
        }

        readonly SteamSharp sharpCore;

        private FilterControl Filter { get;}

        private GRGameRank GameRank { get; set; }

        private Database database { get; }

        private SteamTheme UI { get; }

        private List<Game> ExecuteRecommendation(string steamID)
        {
            Dictionary<string, double> precalculations = ReadFromFile();
            Tuple<List<Game>, List<UserGameTime.Game>> userData = LoadUserGames(steamID);

            Dictionary<int, Game> dbGames = database.FindAllGames();
            dbGames = RemoveGamesWithBlacklistedWords(dbGames);
            dbGames = RemoveDLCFromList(dbGames);
            GameRank = new GRGameRank(dbGames, userData.Item2);

            Debug.WriteLine(precalculations.Count);

            List<Game> RecommenderList = precalculations.Count > 0
                ? GameRank.GetRankedGameList(precalculations)
                : GameRank.GetRankedGameList();

            RecommenderList = Filter.ExecuteFiltering(RecommenderList, userData.Item1);
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

        private Tuple<List<Game>, List<UserGameTime.Game>> LoadUserGames(string steamID)
        {
            List<UserGameTime.Game> userGameList = sharpCore.SteamUserGameTimeListById(steamID);
            List<int> UserGameIDs = userGameList.Select(game => game.appid).ToList();
            List<Game> DBGameList = database.FindGamesById(UserGameIDs).Select(game => game.Value).ToList();

            return new Tuple<List<Game>, List<UserGameTime.Game>>(DBGameList, userGameList);
        }
    }
}
