using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter_System.Filter_Core.Filters.Filter_Pre_Calculations.Models
{
    public class AppWorkClass
    {
        public AppWorkClass(int appID, List<string> tagList)
        {
            AppID = appID;
            TagList = tagList;
            Score = 0;

        }

        public int AppID { get; private set; }

        public List<string> TagList { get; private set; }

        public double Score { get; set; }

        public List<List<string>> TagCombinationList { get; set; } 
    }
}
