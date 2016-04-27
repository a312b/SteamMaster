using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Filter_System.Filter_Core.Models;
using SteamSharp.steamStore.models;

namespace Filter_System.Filter_Core.Filters
{
    class DiversificationFilter : Filter<SteamStoreGame> //This filter is used to apply diversification to recommendations
    {
        protected override Dictionary<int, double> FilterSort(Dictionary<int, SteamStoreGame> dictionaryToSort)
        {
            throw new NotImplementedException();
        }

        protected Dictionary<int, double> FilterSort()
        {
            return DiversificationForCompleteCollection;
        }

        private Dictionary<int, double> DiversificationForCompleteCollection; //Pre-calculated for the complete collection
    }
}
