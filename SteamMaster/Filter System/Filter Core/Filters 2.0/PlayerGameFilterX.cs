using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseCore.lib.converter.models;
using Filter_System.Filter_Core.Filters_2._0.Models;
using SteamSharpCore.steamUser.models;

namespace Filter_System.Filter_Core.Filters_2._0
{
    class PlayerGameFilterX : FilterBase
    {
        public PlayerGameFilterX()
        {
           
        }

        public PlayerGameFilterX(double filterWeight) : base(filterWeight)
        {
            
        }

        protected Dictionary<int, double> FilterSort(List<UserGameTime.Game> gamesToSort)
        {
            return gamesToSort.ToDictionary(game => game.appid, game => currentFilter(game));
        }

        private Func<UserGameTime.Game, double> currentFilter;

        public void UserPlayTimeForever()
        {
            currentFilter = game => game.playtime_forever;
        }

        public void UserPlayTime2Weeks()
        {
            currentFilter = game => game.playtime_2weeks;
        }

        protected override Dictionary<int, double> FilterSort(List<Game> gamesToSort)
        {
            throw new NotImplementedException("");
        }
    }
}
