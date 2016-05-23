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
            GamesToFilterValue = 1;
            MostOwnedValue = 1;
            AvgPlayedForeverValue = 0;
            AvgPlayTime2WeeksValue = 0;
            MetaCriticValue = 1;
        }

        public double GamesToFilterValue { get; set; }

        public double MostOwnedValue { get; set; }

        public double AvgPlayedForeverValue { get; set; }

        public double AvgPlayTime2WeeksValue { get; set; }

        public double MetaCriticValue { get; set; }


        public List<Game> ExecuteFiltering(List<Game> gamesToFilter, List<Game> userGames)
        {
                gamesToFilter.Sort((x, y) => x.RecommenderScore.CompareTo(y.RecommenderScore));

                int score = 1;
                foreach (var game in gamesToFilter)
                {
                    game.RecommenderScore = score*GamesToFilterValue;
                    score++;
                }

            return FilterManagement(gamesToFilter, userGames);
        }

        private List<Game> FilterManagement(List<Game> InputList, List<Game> userGames)
        {

            double active = MostOwnedValue + AvgPlayedForeverValue + AvgPlayTime2WeeksValue + MetaCriticValue;


            if (active > 0)
            {
                #region FilterExecution

                GameFilterX StandardGameFilter = new GameFilterX();
                RemoveGamesFilter removeGamesRemoval = new RemoveGamesFilter();


                StandardGameFilter.OwnerCount(MostOwnedValue);
                InputList = StandardGameFilter.Execute(InputList);

                StandardGameFilter.AvgPlayTimeForever(AvgPlayedForeverValue);
                InputList = StandardGameFilter.Execute(InputList);

                StandardGameFilter.AvgPlayTime2Weeks(AvgPlayTime2WeeksValue);
                InputList = StandardGameFilter.Execute(InputList);

                StandardGameFilter.MetaCritic(MetaCriticValue);
                InputList = StandardGameFilter.Execute(InputList);

                InputList = removeGamesRemoval.Execute(InputList, userGames);

                InputList.Sort();
                #endregion
            }
            return InputList;
        }
    }
}
