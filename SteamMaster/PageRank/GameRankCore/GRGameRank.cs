using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using MongoDB.Driver.Core.Events;
using SteamSharpCore;
using SteamSharpCore.steamUser.models;

namespace GameRank
    
{
    /// <summary>
    /// Serves as an interface for calculating a ranked list 
    /// of game recommendation based on input games.
    /// </summary>
    public class GRGameRank
    {
        #region Fields

        private List<UserGameTime.Game> _userGames;
        private Dictionary<int, Game> _databaseGames;
        private List<GRGame> _userRecommendations;

        #endregion

        #region Constructor

        public GRGameRank(Dictionary<int, Game> databaseGames, List<UserGameTime.Game> userGames)
        {
            _databaseGames = databaseGames;
            _userGames = userGames;
        }

        #endregion

        #region Methods
        
        public List<Game> GetRankedGameList()
        {
            Start();

            List<Game> recommendations = _userRecommendations.Select(game => _databaseGames[game.AppID]).ToList();
            return recommendations;
        }

        public List<GRGame> DontUseThisOne()
        {
            //This is a test class for getting actual GameRank scores
            Start();
            return _userRecommendations.Take(50).ToList();
        }



        /// <summary>
        /// Starts all the GameRank calculations. Point of no return.
        /// </summary>
        private void Start()
        {

            GRTagGameDictionaries tagsAndGames = new GRTagGameDictionaries(_databaseGames);
            tagsAndGames.Start();
            try
            {
                tagsAndGames = RemoveRedundantTags(tagsAndGames);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Place the file \"Exclude list.txt\" in your My Documents folder");
                Console.WriteLine("This file can be found in the GameRank main folder");
            }

            GRCalculateGameRank unbiasedGameRank = new GRCalculateGameRank(tagsAndGames);
            unbiasedGameRank.Start();

            var userBiasedGameRank = new GRCalculateBiasedGameRank(unbiasedGameRank, _userGames);
            userBiasedGameRank.Start();

            _userRecommendations = new List<GRGame>(userBiasedGameRank.CalculateGameRank.Games.Values.ToList());
            _userRecommendations.Sort();
        }

        /// <summary>
        /// This function removes the tags that I have deemed redundant, either
        /// because they don't really describe a game quality, or because
        /// it is not related to gaming at all. This includes joke tags
        /// </summary>
        /// <param name="tagsAndGames"></param>
        /// <returns></returns>
        private GRTagGameDictionaries RemoveRedundantTags(GRTagGameDictionaries tagsAndGames)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Exclude list.txt";
            StreamReader reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line == null) break;
                if (tagsAndGames.TagDictionary.ContainsKey(line))
                    tagsAndGames.TagDictionary.Remove(line);
            }
            return tagsAndGames;
        }

        #endregion
    }
}
