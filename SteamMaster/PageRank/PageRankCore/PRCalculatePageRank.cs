using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PageRank
{
    class PRCalculatePageRank
    {
        #region Fields

        private double DampeningFactor;
        public Dictionary<int, PRGame> Games;
        public Dictionary<string, PRTag> Tags;
        private double Convergence;
        private int Iterations;

        #endregion

        #region Constructor

        public PRCalculatePageRank(PRTagGameDictionaries tagGameDictionaries, double dampeningFactor = 0.25,
            double convergence = 0.0001, int iterations = 10)
        {
            Games = tagGameDictionaries.GameDictionary;
            Tags = tagGameDictionaries.TagDictionary;
            DampeningFactor = dampeningFactor;
            Convergence = convergence;
            Iterations = iterations;
        }

        #endregion

        #region Methods

        public void Start()
        {
            double currentIteration = Games.Values.Sum(game => game.GamePageRank);

            for (int iteration = 0; iteration < Iterations; iteration++)
            {
                PerformIteration();
                UpdateGamePageRanks();

                //Print for debugging
                Console.WriteLine(currentIteration);

                //Keeps track of the iterations to check the convergence
                double previousIteration = currentIteration;
                currentIteration = Games.Values.Sum(game => game.GamePageRank);

                //Break if convergence requirements are met
                if (previousIteration - currentIteration <= Convergence) break;
            }

            Console.WriteLine($"done");
        }

        private void PerformIteration()
        {
            foreach (PRTag tag in Tags.Values)
            {
                tag.TagPageRank = ComputePageRank(tag);
            }
        }


        //Here the games PageRanks are updated after finished iterations
        public void UpdateGamePageRanks()
        {
            foreach (PRGame game in Games.Values)
            {
                game.GamePageRank = 0;
                List<string> gameTags = game.GameTags;
                //The game's new GamePageRank is the sum of the GamePageRank of its tags
                foreach (string tag in gameTags)
                {
                    if (Tags.ContainsKey(tag)) //Game may contain redundant tags that have been removed
                        game.GamePageRank += Tags[tag].TagPageRank;
                }
            }
        }

        //Tags that don't describe the game or are not meant for games at all are removed in this method.
        //Other tags that we have deemed redundant have also been removed.


        //This is where the magic happens
        private double ComputePageRank(PRTag tag)
        {
            //The first fraction is the random teleportation variable
            double firstFraction = (1 - DampeningFactor)/tag.OutLinks;
            double gamePRSum = 0;
            foreach (var prGame in Games.Values.Where(game => game.GameTags.Contains(tag.Tag)))
            {
                double divisor = AdjustDivisor(prGame)*prGame.OutgoingLinks;
                gamePRSum += prGame.GamePageRank/divisor;
            }
            double tagPageRank = firstFraction + DampeningFactor*gamePRSum;

            return tagPageRank;
        }

        /// <summary>
        /// Placeholder comment. This is going to be awesome, just wait for it.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private double AdjustDivisor(PRGame game)
        {
            return Games.Count;
        }

        #endregion
    }
}