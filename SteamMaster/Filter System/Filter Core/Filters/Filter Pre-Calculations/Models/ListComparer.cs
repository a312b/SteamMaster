using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter_System.Filter_Core.Filters.Filter_Pre_Calculations.Models
{
    public class ListComparer <T> : IEqualityComparer<List<T>> //A class used for implementing IEqualityComparer 
    {
        public bool Equals(List<T> x, List<T> y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(List<T> obj)
        {
            string listCombined = "";
            foreach (var entry in obj)
            {
                listCombined += entry;
            }

            return listCombined.GetHashCode();
        }
    }
}
