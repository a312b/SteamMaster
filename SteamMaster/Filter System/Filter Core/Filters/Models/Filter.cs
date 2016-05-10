using System;
using System.Collections.Generic;

namespace Filter_System.Filter_Core.Models
{
    abstract class Filter <T>
    {
        protected Filter()
        {
            FilterWeight = 1;
        }


        public virtual Dictionary<int, double> Execute(Dictionary<int, T> dictionaryToSort) // Execute FilterSort and applies weight
        {
            Dictionary<int, double> returnDictionary = FilterSort(dictionaryToSort);

            List<int> keys = new List<int>(returnDictionary.Keys);
            foreach (int key in keys)
            {
                double assignValue;
                returnDictionary.TryGetValue(key, out assignValue);

                returnDictionary[key] = assignValue*FilterWeight;
            }

            return returnDictionary;
        }


        protected abstract Dictionary<int, double> FilterSort(Dictionary<int, T> dictionaryToSort);

        // Takes a dictionary with SortParameters related to appID -> dictionaryToSort<appID, sortParameter>
        // Returns a dictionary with value related to appID - the appID with the lowest value is most recommended
        // Inheriting class implement the sort rule and type of sort parameter
        // Is used in Execute

        private double _filterWeight;

        public double FilterWeight // Used to adjust weight of the filter by setting the value between 0-1 - default is 1
        {
            get { return _filterWeight; }
            set
            {
                if(value >= 0 && value <= 1)
                {
                    _filterWeight = value;
                } else
                {
                    throw new ArgumentException("Weight have to be between 0 and 1");
                }
            }
        }
    }
}

