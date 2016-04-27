using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter_System.Filter_Core.Filters.Filter_Pre_Calculations.Models
{
    public class ListComparer : IEqualityComparer<List<string>>
    {
        public bool Equals(List<string> x, List<string> y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(List<string> obj)
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
