using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseCore.lib.converter.models;
using Filter_System.Filter_Core.Filters_2._0;

namespace Filter_System
{
    public class FilterControl
    {
        public FilterControl()
        {
            //Standard settings are intiated
            RecommendationListValue = 1;
            MostOwnedValue = 1;
            AvgPlayedForeverValue = 0;
            AvgPlayTime2WeeksValue = 0;
            MetaCriticValue = 1;
        }

        public double RecommendationListValue { get; set; }

        public double MostOwnedValue { get; set; }

        public double AvgPlayedForeverValue { get; set; }

        public double AvgPlayTime2WeeksValue { get; set; }

        public double MetaCriticValue { get; set; }


        public List<Game> ExecuteFiltering(List<Game> recommendationList, List<Game> userGames)
        {
            if (RecommendationListValue != 0)
            {
                recommendationList.Sort((x, y) => x.RecommenderScore.CompareTo(y.RecommenderScore));

                int score = 1;
                foreach (var game in recommendationList)
                {
                    game.RecommenderScore = score*RecommendationListValue;
                    score++;
                }
            }
            else
            {
                foreach (var game in recommendationList)
                {
                    game.RecommenderScore = 0;
                }
            }

            return FilterManagement(recommendationList, userGames);
        }




        private List<Game> FilterManagement(List<Game> InputList, List<Game> userGames)
        {

            double active = MostOwnedValue + AvgPlayedForeverValue + AvgPlayTime2WeeksValue + MetaCriticValue;


            if (active > 0)
            {
                #region FilterExecution

                GameFilterX StandardGameFilter = new GameFilterX();
                PlayerGameFilterX PlayerGameRemoval = new PlayerGameFilterX();


                StandardGameFilter.OwnerCount(MostOwnedValue);
                InputList = StandardGameFilter.Execute(InputList);

                StandardGameFilter.AvgPlayTimeForever(AvgPlayedForeverValue);
                InputList = StandardGameFilter.Execute(InputList);

                StandardGameFilter.AvgPlayTime2Weeks(AvgPlayTime2WeeksValue);
                InputList = StandardGameFilter.Execute(InputList);

                StandardGameFilter.MetaCritic(MetaCriticValue);
                InputList = StandardGameFilter.Execute(InputList);

                InputList = PlayerGameRemoval.Execute(InputList, userGames);

                InputList.Sort();
                #endregion
            }
            return InputList;
        }
    }
}
