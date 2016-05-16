using System.Collections.Generic;
using System.Linq;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using SteamSharpCore.steamUser.models;

namespace GameRank
{
    /// <summary>
    ///     In this class the previously calculated rank for each tag is
    ///     adjusted with respect to the input steam user based on the
    ///     frequency of each tag in the users game library.
    ///     Afterwards the rank of each game is updated accordingly
    /// </summary>
    class GRCalculateBiasedGameRank
    {
        #region Constructor

        public GRCalculateBiasedGameRank(GRCalculateGameRank calculateGameRank, List<UserGameTime.Game> userGames)
        {
            _userGames = userGames;
            CalculateGameRank = calculateGameRank;
        }

        #endregion

        #region Fields

        public GRCalculateGameRank CalculateGameRank { get; }

        private GRTagGameDictionaries _tagGameDictionaries;
        private Dictionary<int, GRGame> _userGameDictionary;
        private readonly List<UserGameTime.Game> _userGames;

        #endregion

        #region Methods

        /// <summary>
        ///     Some preliminary calculations are performed here. A user game
        ///     dictionary is made so that it can be uesd as input for generating
        ///     tag and game dictionaries for the purpose of counting tag frequency
        ///     which is needed for the BiasTagGameRank function.
        /// </summary>
        public void Start()
        {
            _userGameDictionary = GenerateUserGameDictionary();
            _tagGameDictionaries = GenerateTagGameDictionaries(_userGameDictionary);
            _tagGameDictionaries.Start();
            BiasTagGameRank();
        }

        private Dictionary<int, GRGame> GenerateUserGameDictionary()
        {
            Dictionary<int, GRGame> userGameDictionary = new Dictionary<int, GRGame>();
            foreach (UserGameTime.Game game in _userGames) //.Where(game => game.playtime_forever > 0))
            {
                if (CalculateGameRank.Games.ContainsKey(game.appid))
                {
                    GRGame userGame = CalculateGameRank.Games[game.appid];
                    userGameDictionary.Add(game.appid, userGame);
                    CalculateGameRank.Games.Remove(game.appid);
                }
            }
            return userGameDictionary;
        }

        private GRTagGameDictionaries GenerateTagGameDictionaries(Dictionary<int, GRGame> userGameDictionary)
        {
            Database db = new Database();
            Dictionary<int, Game> userGames =
                db.FindGamesById(userGameDictionary.Values.Select(item => item.AppID).ToList());
            GRTagGameDictionaries tagGameDictionaries = new GRTagGameDictionaries(userGames);

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
                    prTag.GameRank = prTag.GameRank*tagFrequency;
                    continue;
                }
                prTag.GameRank = 1.0/CalculateGameRank.Games.Count;
            }

            CalculateGameRank.UpdateGameRanks();
        }

        private int GetTagOutlinks(GRTag grTag)
        {
            //Gets the number of outlinks for a tag based on the corresponding entry in the tag dictionary.
            return _tagGameDictionaries.TagDictionary[grTag.Tag].Outlinks;
        }

        #endregion
    }
}