using System;
using System.Collections.Generic;
using System.Linq;

namespace GameRank
{
    class GRCalculateGameRank
    {
        #region Fields

        public Dictionary<int, GRGame> Games { get; set; }
        public Dictionary<string, GRTag> Tags { get; set; }

        private readonly double _dampingFactor;
        private readonly double _convergence;
        private readonly int _iterations;

        #endregion

        #region Constructor

        public GRCalculateGameRank(GRTagGameDictionaries tagGameDictionaries, double dampingFactor = 0.25,
            double convergence = 0.0001, int iterations = 100)
        {
            Games = tagGameDictionaries.GameDictionary;
            Tags = tagGameDictionaries.TagDictionary;
            _dampingFactor = dampingFactor;
            _convergence = convergence;
            _iterations = iterations;
        }

        public GRCalculateGameRank(GRTagGameDictionaries tagGameDictionaries,
            Dictionary<string, double> precalculatedGameRankDictionary)
        {
            Games = tagGameDictionaries.GameDictionary;
            Tags = tagGameDictionaries.TagDictionary;

            foreach (var entry in precalculatedGameRankDictionary)
            {
                Tags[entry.Key].GameRank = entry.Value;
            }
            UpdateGameRanks();
        }

        #endregion

        #region Methods

        public void Start()
        {
            double currentIteration = Games.Values.Sum(game => game.GameRank);

            for (int iteration = 0; iteration < _iterations; iteration++)
            {
                PerformIteration();
                UpdateGameRanks();

                //Print for debugging
                Console.WriteLine(currentIteration);

                //Keeps track of the iterations to check the convergence
                double previousIteration = currentIteration;
                currentIteration = Games.Values.Sum(game => game.GameRank);

                //Break if convergence requirements are met
                if (previousIteration - currentIteration <= _convergence) break;
            }
        }

        private void PerformIteration()
        {
            foreach (GRTag tag in Tags.Values)
            {
                tag.GameRank = ComputeGameRank(tag);
            }
        }


        //Here the games GameRanks are updated after finished iterations
        public void UpdateGameRanks()
        {
            foreach (GRGame game in Games.Values)
            {
                game.GameRank = 0;
                //The game's new GameRank is the sum of the GameRank of its tags
                foreach (string tag in game.Tags)
                {
                    if (this.Tags.ContainsKey(tag)) //Game may contain redundant tags that have been removed
                        game.GameRank += Tags[tag].GameRank;
                }
            }
        }

        //Tags that don't describe the game or are not meant for games at all are removed in this method.
        //Other tags that we have deemed redundant have also been removed.


        //This is where the magic happens
        private double ComputeGameRank(GRTag tag)
        {
            //The first fraction is the random teleportation variable
            double firstFraction = (1 - _dampingFactor)/tag.Outlinks;
            double gamePRSum = 0;
            foreach (var prGame in Games.Values.Where(game => game.Tags.Contains(tag.Tag)))
            {
                double divisor = AdjustDivisor(prGame)*prGame.Outlinks;
                gamePRSum += prGame.GameRank/divisor;
            }
            double tagGameRank = firstFraction + _dampingFactor*gamePRSum;

            return tagGameRank;
        }

        /// <summary>
        ///     Placeholder comment. This is going to be awesome, just wait for it.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private double AdjustDivisor(GRGame game)
        {
            return Games.Count;
        }

        #endregion
    }
}