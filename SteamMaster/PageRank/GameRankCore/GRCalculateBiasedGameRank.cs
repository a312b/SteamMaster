using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using SteamSharpCore;
using SteamSharpCore.steamUser.models;

namespace GameRank
{
    /// <summary>
    /// In this class the previously calculated rank for each tag is
    /// adjusted with respect to the input steam user based on the
    /// frequency of each tag in the users game library.
    /// Afterwards the rank of each game is updated accordingly
    /// </summary>
    class GRCalculateBiasedGameRank
    {
        #region Public Fields

        public GRCalculateGameRank CalculateGameRank;

        #endregion

        #region Private Fields

        private GRTagGameDictionaries _tagGameDictionaries;
        private Dictionary<int, GRUserGame> _userGameDictionary;
        private readonly List<UserGameTime.Game> _userGames;

        #endregion

        #region Constructor

        public GRCalculateBiasedGameRank(GRCalculateGameRank calculateGameRank, List<UserGameTime.Game> userGames)
        {
            _userGames = userGames;
            CalculateGameRank = calculateGameRank;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Some preliminary calculations are performed here. A user game 
        /// dictionary is made so that it can be uesd as input for generating
        /// tag and game dictionaries for the purpose of counting tag frequency
        /// which is needed for the BiasTagGameRank function.
        /// </summary>
        public void Start()
        {
            _userGameDictionary = GenerateUserGameDictionary();
            _tagGameDictionaries = GenerateTagGameDictionaries(_userGameDictionary);
            _tagGameDictionaries.Start();
            BiasTagGameRank();

        }

        private Dictionary<int, GRUserGame> GenerateUserGameDictionary()
        {
            Dictionary<int, GRUserGame> userGameDictionary = new Dictionary<int, GRUserGame>();
            foreach (UserGameTime.Game game in _userGames)//.Where(game => game.playtime_forever > 0))
            {
                if (CalculateGameRank.Games.ContainsKey(game.appid))
                {
                    GRUserGame userGame = new GRUserGame(CalculateGameRank.Games[game.appid], game);
                    userGameDictionary.Add(game.appid, userGame);
                    CalculateGameRank.Games.Remove(game.appid);
                }
            }
            return userGameDictionary;
        }

        private GRTagGameDictionaries GenerateTagGameDictionaries(Dictionary<int, GRUserGame> userGameDictionary)
        {
            Database db = new Database();
            Dictionary<int, Game> gameIDs =
                db.FindGamesById(userGameDictionary.Values.Select(item => item.AppID).ToList());
            GRTagGameDictionaries tagGameDictionaries = new GRTagGameDictionaries(gameIDs);

            return tagGameDictionaries;
        }

        
        private void BiasTagGameRank()
        {
            //This is where the value of each tag is adjusted according to the user
            //Afterwards the rank of each game is updated accordingly

            //CalculateMultiplier(); -- Changed algorithm. This function is now obsolete
            //leaving it here just in case

            foreach (GRTag prTag in CalculateGameRank.Tags.Values)
            {
                if (_tagGameDictionaries.TagDictionary.ContainsKey(prTag.Tag))
                {
                    int tagFrequency = GetTagOutlinks(prTag);
                    prTag.GameRank = prTag.GameRank * tagFrequency;
                    continue;
                }
                prTag.GameRank = 1.0/CalculateGameRank.Games.Count;
            }

            CalculateGameRank.UpdateGameGameRanks();
        }

        private void CalculateMultiplier()
        {
            int minimumOutlinks = _tagGameDictionaries.TagDictionary.Values.Min(tag => tag.OutLinks);
            int maximumOutlinks = _tagGameDictionaries.TagDictionary.Values.Max(tag => tag.OutLinks);
            
            int variance = maximumOutlinks - minimumOutlinks;

            //This is the start value for the tag weight modifier
            double multiplier = 0.1;

            //This is the amount the multiplier is incremented every time
            //outlinks is increased by 1. With the current value,
            //the tag with the most outlinks is multiplied by a number
            //10 factors higher than the tag with the least outlinks
            double increment = 0.9/variance;

            //This is a dictionary containing the number of outlinks as a key,
            //and the multiplier as a value.
            Dictionary<int, double> tagMultipliers = new Dictionary<int, double>();

            for (int outlinks = minimumOutlinks; outlinks <= maximumOutlinks; outlinks++)
            {
                tagMultipliers.Add(outlinks, multiplier += increment);
            }

            //In this loop, each tag that exists in the user's library is multiplied by
            //the value specified by its number of outlinks. If it does not exist in the user's library,
            //its value is set to one divided by the total number of games.
            foreach (GRTag prTag in CalculateGameRank.Tags.Values)
            {
                if (_tagGameDictionaries.TagDictionary.ContainsKey(prTag.Tag))
                {
                    int outlinks = GetTagOutlinks(prTag);
                    //double tagMultiplier = tagMultipliers[outlinks];
                    double tagMultiplier = outlinks;
                    prTag.GameRank = prTag.GameRank*tagMultiplier;
                    continue;
                }
                prTag.GameRank = (double)1/CalculateGameRank.Games.Count;
                
            }
        }

        private int GetTagOutlinks(GRTag grTag)
        {
            //Gets the number of outlinks for a tag based on the corresponding entry in the tag dictionary.
            return _tagGameDictionaries.TagDictionary[grTag.Tag].OutLinks;
        }

        #endregion

    }
}