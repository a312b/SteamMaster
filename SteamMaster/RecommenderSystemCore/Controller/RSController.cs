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
            List<Game> RecommenderList = new List<Game>();
            UserWorkClass User = new UserWorkClass(steamID);

            Dictionary<int, Game> dbGames = database.FindAllGames();
            dbGames = RemoveGamesWithBlacklistedWords(dbGames);
            GameRank = new GRGameRank(dbGames, User.userListGameList);
            RecommenderList = GameRank.GetRankedGameList(precalculations);
            //RecommenderList = GameRank.GetRankedGameList();

            //RecommenderList = FilterManagement(RecommenderList, User);

            return RecommenderList;
        }

        private Dictionary<string, double> ReadFromFile()
        {
            //File can be found in the main GameRank folder. Should be placed in your My Documents folder
            string path = Directory.GetCurrentDirectory() + @"\TagsAndRanks.txt";
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

        private List<Game> FilterManagement(List<Game> ListToFilter, UserWorkClass User)
        {
            //Controls the weight of the filters in PopularityFilter
            double MostOwnedValue = 1;
            double AvgPlayedForeverValue = 0;
            double AvgPlayTime2WeeksValue = 0;

            GameFilterX StandardGameFilter = new GameFilterX();
            PlayerGameFilterX PlayerGameRemoval = new PlayerGameFilterX();
            


            StandardGameFilter.OwnerCount(MostOwnedValue);
            ListToFilter = StandardGameFilter.Execute(ListToFilter);

            StandardGameFilter.AvgPlayTimeForever(AvgPlayedForeverValue);
            ListToFilter = StandardGameFilter.Execute(ListToFilter);

            StandardGameFilter.AvgPlayTime2Weeks(AvgPlayTime2WeeksValue);
            ListToFilter = StandardGameFilter.Execute(ListToFilter);

            ListToFilter = PlayerGameRemoval.Execute(ListToFilter, User.DBGameList);
        
            return ListToFilter;
        }


        //private void RecommendGameList(string steamID) // fix senere - mere funktion findes i funktionen GenerateGameList() under UI;
        //{
        //    SteamSharpCore.SteamSharp _steamSharpTest = new SteamSharpCore.SteamSharp("");
        //    int roundCount = 0;
        //    string steamId = UI.steamIdTextBox.Text.ToLower(); //not used
        //    string[] idArray =
        //    {
        //        "340", "280", "570", "80", "240", "400", "343780", "500", "374320", "10500", "252950", "300", "7940",
        //        "10180"
        //    };
        //    //150, 22380, 377160 == nullreference på htmlagility 
        //    //340, 570 == nullreference på prisen (der findes f.eks. "price_in_cents_with_discount" i stedet)

        //    List<SteamStoreGame> formGameList = _steamSharpTest.GameListByIds(idArray);
        //    if (formGameList != null)
        //    {
        //        UI.ClearGameListBox();
        //        foreach (SteamStoreGame game in formGameList)
        //        {
        //            //ThreadPool.QueueUserWorkItem(LoadHeaderImages(game.data.steam_appid, roundCount));
        //            UI.LoadHeaderImages(game.data.steam_appid, roundCount);
        //            //UI.LoadGameInfo(game, roundCount);
        //            roundCount++;
        //        }
        //    }
        //}


    }
}
