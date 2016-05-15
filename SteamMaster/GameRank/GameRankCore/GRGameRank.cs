using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DatabaseCore.lib.converter.models;
using SteamSharpCore.steamUser.models;

namespace GameRank

{
    /// <summary>
    ///     Serves as an interface for calculating a ranked list
    ///     of game recommendation based on input games.
    /// </summary>
    public class GRGameRank
    {
        #region Constructor

        public GRGameRank(Dictionary<int, Game> databaseGames, List<UserGameTime.Game> userGames)
        {
            _databaseGames = databaseGames;
            _userGames = userGames;
        }

        #endregion

        #region Fields

        private readonly List<UserGameTime.Game> _userGames;
        private readonly Dictionary<int, Game> _databaseGames;
        private List<GRGame> _userRecommendations;

        #endregion

        #region Methods

        public List<Game> GetRankedGameList()
        {
            Start();

            List<Game> recommendations = new List<Game>();
            foreach (var game in _userRecommendations)
            {
                Game dbGame = _databaseGames[game.AppID];
                dbGame.RecommenderScore = game.GameRank;
                recommendations.Add(dbGame);

            }
            return recommendations;
        }

        /// <summary>
        ///     Use this overloaded method if you already calculated the GameRank for each tag
        /// </summary>
        /// <param name="precalculatedTagGameRank"></param>
        /// <returns></returns>
        public List<Game> GetRankedGameList(Dictionary<string, double> precalculatedTagGameRank)
        {
            Start(precalculatedTagGameRank);
            List<Game> recommendations = new List<Game>();
            foreach (var game in _userRecommendations)
            {
                Game dbGame = _databaseGames[game.AppID];
                dbGame.RecommenderScore = game.GameRank;
                recommendations.Add(dbGame);
                
            }
            return recommendations;
        }


        /// <summary>
        ///     If no input parameters are given it calculates the GameRank network. Otherwise it uses the precalculations
        /// </summary>
        private void Start(Dictionary<string, double> precalculatedTagGameRank = null)
        {
            GRTagGameDictionaries tagsAndGames = new GRTagGameDictionaries(_databaseGames);
            tagsAndGames.Start();

            GRCalculateGameRank unbiasedGameRank = precalculatedTagGameRank == null
                ? Calculate(tagsAndGames)
                : new GRCalculateGameRank(tagsAndGames, precalculatedTagGameRank);
            //GRCalculateGameRank unbiasedGameRank = new GRCalculateGameRank(tagsAndGames, precalculatedTagGameRank);

            var userBiasedGameRank = new GRCalculateBiasedGameRank(unbiasedGameRank, _userGames);
            userBiasedGameRank.Start();

            _userRecommendations = new List<GRGame>(userBiasedGameRank.CalculateGameRank.Games.Values.ToList());
            _userRecommendations.Sort();
        }

        private GRCalculateGameRank Calculate(GRTagGameDictionaries tagsAndGames)
        {
            GRCalculateGameRank unbiasedGameRank = new GRCalculateGameRank(tagsAndGames);
            unbiasedGameRank.Start();
            return unbiasedGameRank;
        }

        #endregion
    }
}