using System.Collections.Generic;
using DatabaseCore.lib.converter.models;

namespace Filter_System.Filter_Core.Filters_2._0.Models
{
    public abstract class FilterBase
    {
        protected FilterBase()
        {
            FilterWeight = 1;
        }

        protected FilterBase(double filterWeight)
        {
            FilterWeight = filterWeight;
        }


        public double FilterWeight { get; set; } // Used to adjust weight of the filter by setting the value - default is 1


        public virtual List<Game> Execute(List<Game> gamesToSort)
        {
            List<Game> workList = new List<Game>(gamesToSort);

            Dictionary<int, double> scoreDictionary = FilterSort(workList);

            foreach (var game in gamesToSort)
            {
                game.RecommenderScore += scoreDictionary[game.SteamAppId]*FilterWeight;
            }
            
            gamesToSort.Sort();

            return gamesToSort;
        }

        //FilterSort is the method that sort the list of games. It is implemented in the classes inheriting from FilterBase
        protected abstract Dictionary<int, double> FilterSort(List<Game> gamesToSort);
    }
}
