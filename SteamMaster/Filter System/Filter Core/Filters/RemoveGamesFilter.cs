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
    public class RemoveGamesFilter : FilterBase
    {
        public RemoveGamesFilter()
        {
            FilterWeight = 0;
        }

        private List<Game> UserGames { get; set; }

        protected override Dictionary<int, double> FilterSort(List<Game> gamesToSort)
        {
            Dictionary<int, double> returnDictionary = new Dictionary<int, double>();

            int value = 1;
            foreach (var game in gamesToSort)
            {
                returnDictionary.Add(game.SteamAppId, value);
                value++;
            }


            if (UserGames != null)
            {
                foreach (var game in UserGames)
                {
                    int appID = game.SteamAppId;

                    if (returnDictionary.ContainsKey(appID))
                    {
                        returnDictionary.Remove(appID);
                    }
                }
            }

            return returnDictionary;


        }


        public List<Game> Execute(List<Game> gamesToSort, List<Game> userGames)
        {
            UserGames = userGames;
            return base.Execute(gamesToSort);
        }

        public override List<Game> Execute(List<Game> gamesToSort)
        {
            FilterWeight = 0;
            return gamesToSort;
        }

    }
}
