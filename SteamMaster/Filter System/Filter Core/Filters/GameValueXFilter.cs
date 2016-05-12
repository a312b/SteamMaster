using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseCore.lib.converter.models;
using Filter_System.Filter_Core.Filters.Filter_Pre_Calculations.Models;
using Filter_System.Filter_Core.Models;

namespace Filter_System.Filter_Core.Filters
{
    
    public class GameValueXFilter : Filter<Game>
    {
        public GameValueXFilter()
        {
            OwnerCount();
        }

        public GameValueXFilter(double filterWeight) : base(filterWeight)
        {
            OwnerCount();
        }

        private Func<Game, double> activeFilter; //Change the activeFilter by executing one of the methods for it. Standard is set for OwnerCount.

        protected override Dictionary<int, double> FilterSort(Dictionary<int, Game> dictionaryToSort)
        {
            Dictionary<int, double> returnDictionary = new Dictionary<int, double>();
            List<AppWorkClass> rankingList = dictionaryToSort.Select(game => new AppWorkClass(game.Key, activeFilter(game.Value))).ToList();
            rankingList = rankingList.OrderByDescending(x => x).ToList();
            
            int i = 1;

            foreach (var game in rankingList)
            {
                returnDictionary.Add(game.AppID, i);
                i++;
            }

            return returnDictionary;
        }

        public Dictionary<int, double> Execute(List<Game> listToSort)
        {
            Dictionary<int, Game> dictionaryToSort = listToSort.ToDictionary(game => game.SteamAppId);
            return Execute(dictionaryToSort);
        }


        public void AvgPlayTime2Weeks()
        {
            activeFilter = game => game.AveragePlayTime2Weeks;
        }

        public void AvgPlayTimeForever()
        {
            activeFilter = game => game.AveragePlayTime;

        }

        public void OwnerCount()
        {
            activeFilter = game => game.OwnerCount;
        }

        public void AgeRating()
        {
            activeFilter = game => game.AgeRating;
        }

        public void Price()
        {
            activeFilter = game => game.Price;
        }

    }
}
