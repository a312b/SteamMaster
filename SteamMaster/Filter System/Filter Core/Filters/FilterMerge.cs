using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter_System.Filter_Core.Models
{
    class FilterMerge : Filter<double> //Class for merging the values of dictionaries returned from different filters
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
    }
}
