using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Filter_System.Filter_Core.Filters.Filter_Pre_Calculations.Models;
using Filter_System.Filter_Core.Models;
using SteamSharpCore.steamUser.models;

namespace Filter_System.Filter_Core.Filters
{
    public class PlayerGameXFilter : Filter<UserGameTime.Game>
    {
        public PlayerGameXFilter()
        {
            UserPlayTimeForever();
        }

        public PlayerGameXFilter(double filterWeight) : base(filterWeight)
        {
            UserPlayTimeForever();
        }

        private Func<UserGameTime.Game, double> currentFilter; 

        protected override Dictionary<int, double> FilterSort(Dictionary<int, UserGameTime.Game> dictionaryToSort)
        {

            Dictionary<int, double> returnDictionary = new Dictionary<int, double>();
            List<AppWorkClass> rankingList = dictionaryToSort.Select(game => new AppWorkClass(game.Key, currentFilter(game.Value))).ToList();
            rankingList = rankingList.OrderByDescending(x => x).ToList();

            int i = 1;

            foreach (var game in rankingList)
            {
                returnDictionary.Add(game.AppID, i);
                i++;
            }

            return returnDictionary;
        }

        public Dictionary<int, double> Execute(List<UserGameTime.Game> listToSort)
        {
            Dictionary<int, UserGameTime.Game> dictionaryToSort = listToSort.ToDictionary(game => game.appid);
            return Execute(dictionaryToSort);
        }

        public void UserPlayTimeForever()
        {
            currentFilter = game => game.playtime_forever;
        }

        public void UserPlayTime2Weeks()
        {
            currentFilter = game => game.playtime_2weeks;
        }
    }
}
