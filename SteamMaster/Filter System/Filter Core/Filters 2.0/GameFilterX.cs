using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseCore.lib.converter.models;
using Filter_System.Filter_Core.Filters_2._0.Models;

namespace Filter_System.Filter_Core.Filters_2._0
{
    class GameFilterX : FilterBase
    {
        public GameFilterX()
        {
            OwnerCount();
        }

        public GameFilterX(double filterWeight) : base(filterWeight)
        {
            OwnerCount();
        }

        protected override Dictionary<int, double> FilterSort(List<Game> gamesToSort)
        {
            return gamesToSort.ToDictionary(game => game.SteamAppId, game => activeFilter(game));
        }

        private Func<Game, double> activeFilter; //Change the activeFilter by executing one of the methods for it. Standard is set for OwnerCount.

        public void AvgPlayTime2Weeks()
        {
            activeFilter = game => game.AveragePlayTime2Weeks;
        }

        public void AvgPlayTimeForever()
        {
            activeFilter = game => game.AveragePlayTime;

        }

        public void OwnerCount()
        {
            activeFilter = game => game.OwnerCount;
        }

        public void AgeRating()
        {
            activeFilter = game => game.AgeRating;
        }

        public void Price()
        {
            activeFilter = game => game.Price;
        }
    }
}
