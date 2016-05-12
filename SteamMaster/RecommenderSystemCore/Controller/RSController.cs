using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using Filter_System.Filter_Core.Filters;
using Filter_System.Filter_Core.Models;
using PageRank;
using RecommenderSystemCore.User_Data_Handling.Models;
using SteamSharpCore.steamStore.models;
using SteamSharpCore;
using SteamUI;

namespace RecommenderSystemCore.Controller
{
    class RSController
    {
        public RSController(SteamTheme ui)
        {


            database = new Database();
            UI = ui;

            //   ui.RecommendButtomClick += 

        }

        private PRGameRanker PageRank { get; set; }

        private Database database { get; set; }

        SteamTheme UI { get; set; }

        public void Start()
        {
            UI.Show();
            
        }

        private List<int> ExecuteRecommendation(string steamID)
        {
            UserWorkClass User = new UserWorkClass(steamID);
            List<int> returnList = new List<int>();
            Dictionary<int, Game> dbGames = database.FindAllGames();

            PageRank = new PRGameRanker(dbGames, User.userListGameList);
            List<Game> PageRankList = PageRank.GetRankedGameList();


        }

        private Dictionary<int, double> FilterManagement(Dictionary<int, Game> dictionaryToFilter)
        {
            //Controls the weight of the filters in PopularityFilter
            double MostPlayedValue = 1;
            double MostPlayed2WeeksValue = 1;
            //Controls the weight of the PopularityFilter as a whole
            double PopularityValue = 1;


            Dictionary<int, double> MostPlayedFilter = new GameValueXFilter(MostPlayedValue).Execute(dictionaryToFilter);
            Dictionary<int, double> MostPlayed2WeeksFilter =
                new GameValueXFilter(MostPlayed2WeeksValue).Execute(dictionaryToFilter);

            Dictionary<int, double> PopularityFilter = new FilterMerge(PopularityValue).Execute(MostPlayed2WeeksFilter, MostPlayedFilter);



        }

        private Dictionary<int, double> FilterManagement(List<Game> listToFilter)
        {
            Dictionary<int, Game> dictionaryToFilter = listToFilter.ToDictionary(game => game.SteamAppId);

            return FilterManagement(dictionaryToFilter);
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
