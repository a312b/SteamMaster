using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseCore.lib.converter.models;
using Filter_System.Filter_Core.Filters_2._0.Models;

namespace Filter_System.Filter_Core.Filters_2._0
{
    public class GameFilterX : FilterBase
    {
        public GameFilterX()
        {
            OwnerCount(1);
        }

        public GameFilterX(double filterWeight) : base(filterWeight)
        {
            OwnerCount(1);
        }

        protected override Dictionary<int, double> FilterSort(List<Game> gamesToSort)
        {
            return gamesToSort.ToDictionary(game => game.SteamAppId, game => activeFilter(game));
        }

        public virtual List<Game> Execute(List<Game> gamesToSort, double filterWeight)
        {
            FilterWeight = filterWeight;
            return Execute(gamesToSort);
        }

        private Func<Game, double> activeFilter; //Change the activeFilter by executing one of the methods for it. Standard is set for OwnerCount.

        public void AvgPlayTime2Weeks(double filterWeight)
        {
            activeFilter = game => game.AveragePlayTime2Weeks;
        }

        public void AvgPlayTimeForever(double filterWeight)
        {
            activeFilter = game => game.AveragePlayTime;
            FilterWeight = filterWeight;
        }

        public void OwnerCount(double filterWeight)
        {
            activeFilter = game => game.OwnerCount;
            FilterWeight = filterWeight;
        }

        public void AgeRating(double filterWeight)
        {
            activeFilter = game => game.AgeRating;
            FilterWeight = filterWeight;
        }

        public void Price(double filterWeight)
        {
            activeFilter = game => game.Price;
            FilterWeight = filterWeight;
        }
    }
}
