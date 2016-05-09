using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseCore.lib.converter.models;
using Filter_System.Filter_Core.Filters.Filter_Pre_Calculations.Models;
using Filter_System.Filter_Core.Models;

namespace Filter_System.Filter_Core.Filters
{
    class PopularityFilter : Filter<Game>
    {
        protected override Dictionary<int, double> FilterSort(Dictionary<int, Game> dictionaryToSort)
        {
            Dictionary<int, double> returnDictionary = new Dictionary<int, double>();
            List<AppWorkClass> rankingList = dictionaryToSort.Select(game => new AppWorkClass(game.Key, game.Value.OwnerCount)).ToList();
            rankingList = rankingList.OrderByDescending(x => x).ToList();
            
            int i = 1;

            foreach (var game in rankingList)
            {
                returnDictionary.Add(game.AppID, i);
                i++;
            }

            foreach (var game in rankingList)
            {
                Console.WriteLine($"ID: {game.AppID} Score: {game.Score}");
            }

            return returnDictionary;
        }
    }
}
