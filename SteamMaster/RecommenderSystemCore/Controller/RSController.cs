using System.Collections.Generic;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using Filter_System.Filter_Core.Filters_2._0;
using PageRank;
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

            UI.RecommendButtomClick += ExecuteRecommendation;

        }

        private PRGameRanker PageRank { get; set; }

        private Database database { get; set; }

        SteamTheme UI { get; set; }

        public void Start()
        {
            UI.Show(); 
        }

        private List<Game> ExecuteRecommendation(string steamID)
        {
            List<Game> RecommenderList = new List<Game>();
            UserWorkClass User = new UserWorkClass(steamID);

            Dictionary<int, Game> dbGames = database.FindAllGames();
            PageRank = new PRGameRanker(dbGames, User.userListGameList);
            RecommenderList = PageRank.GetRankedGameList();

            RecommenderList = FilterManagement(RecommenderList, User);

            return RecommenderList;
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
