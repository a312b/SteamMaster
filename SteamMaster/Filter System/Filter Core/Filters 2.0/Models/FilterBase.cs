using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        private double _filterWeight;

        public double FilterWeight // Used to adjust weight of the filter by setting the value between 0-1 - default is 1
        {
            get { return _filterWeight; }
            set
            {
                if (value >= 0 && value <= 1)
                {
                    _filterWeight = value;
                }
                else
                {
                    throw new ArgumentException("Weight have to be between 0 and 1");
                }
            }
        }


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

        protected abstract Dictionary<int, double> FilterSort(List<Game> gamesToSort);
    }
}
