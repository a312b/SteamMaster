using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseCore.lib.converter.models;

namespace Filter_System.Filter_Core.Models
{
    public class FilterMerge : Filter<double> //Class for merging the values of dictionaries returned from different filters
    {
        protected override Dictionary<int, double> FilterSort(Dictionary<int, double> dictionaryToSort)
        {
            return dictionaryToSort;
        }

        protected Dictionary<int, double> FilterSort(params Dictionary<int, double>[] dictionaryToSort) // If more than one dictionary is entered, then overload FilterSort
        {                                                                                               // will accumulate the values for each app ID
            List<int> keys = new List<int>(dictionaryToSort[1].Keys);
            Dictionary<int, double> returnDictionary = keys.ToDictionary<int, int, double>(key => key, key => 0);


            foreach (var dictionary in dictionaryToSort)
            {
                foreach (int key in keys)
                {
                    double Value;
                    dictionary.TryGetValue(key, out Value);

                    returnDictionary[key] += Value;
                }
            }

            return returnDictionary;
        }

        public Dictionary<int, double> Execute(params Dictionary<int, double>[] dictionaryToSort) //Overloaded to make params Dictionaries, besides that equal to base
        {                                                                                         //except dictionaryToSort is an array of dictionaries
            Dictionary<int, double> returnDictionary = FilterSort(dictionaryToSort);

            List<int> keys = new List<int>(returnDictionary.Keys);

            foreach (int key in keys)
            {
                double assignValue;
                returnDictionary.TryGetValue(key, out assignValue);

                returnDictionary[key] = assignValue * FilterWeight;
            }

            return returnDictionary;
        }

        public Dictionary<int, double> Execute(params List<Game>[] listToSort)
        {
            List<Dictionary<int, double>> dictionaryCollection = new List<Dictionary<int, double>>();

            foreach (var list in listToSort)
            {
                Dictionary<int, double> workDictionary = new Dictionary<int, double>();
                int value = 1;
                foreach (var Game in list)
                {
                    workDictionary.Add(Game.SteamAppId, value);
                    value++;
                }

                dictionaryCollection.Add(workDictionary);

            }

            return Execute(dictionaryCollection.ToArray());
        }
    }
}
