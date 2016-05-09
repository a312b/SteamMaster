using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter_System.Filter_Core.Filters.Filter_Pre_Calculations.Models
{
    public class AppWorkClass : IComparable<AppWorkClass>
    {
        public AppWorkClass(int appID, List<string> tagList, double score)
        {
            AppID = appID;
            TagList = tagList;
            Score = score;

        }

        public AppWorkClass(int appID, double score) : this(appID, new List<string>(), score)
        {

        }

        public AppWorkClass(int appID, List<string> tagList) : this(appID, tagList, 0)
        {

        }

        public AppWorkClass(int appID) : this(appID, new List<string>(), 0)
        {
            
        }

        public int AppID { get; private set; }

        public List<string> TagList { get; private set; }

        public double Score { get; set; }

        public List<List<string>> TagCombinationList { get; set; }

        public int CompareTo(AppWorkClass other) //Sort after the score of the App
        {
            int sourceScore = Convert.ToInt32(Score);
            int compareScore = Convert.ToInt32(other.Score);
            return sourceScore - compareScore;
        }
        
    }
}
