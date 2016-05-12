using System.Collections.Generic;
using System.Linq;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using SteamSharpCore;
using SteamSharpCore.steamUser.models;

namespace PageRank
{
    /// <summary>
    /// In this class the previously calculated rank for each tag is
    /// adjusted with respect to the input steam user based on the
    /// frequency of each tag in the users game library.
    /// Afterwards the rank of each game is updated accordingly
    /// </summary>
    class PRCalculatePersonalizedPageRank
    {
        #region Public Fields

        public PRCalculatePageRank CalculatePageRank;

        #endregion

        #region Private Fields

        private PRTagGameDictionaries _tagGameDictionaries;
        private Dictionary<int, PRUserGame> _userGameDictionary;
        private List<UserGameTime.Game> _userGames;
        private SteamSharp _steamSharp;

        #endregion

        #region Constructor

        public PRCalculatePersonalizedPageRank(PRCalculatePageRank calculatePageRank, List<UserGameTime.Game> userGames)
        {
            _userGames = userGames;
            CalculatePageRank = calculatePageRank;
        }

        #endregion

        #region Methods
        public void Start()
        {
            _userGameDictionary = GenerateUserGameDictionary();
            _tagGameDictionaries = GenerateTagGameDictionaries(_userGameDictionary);
            _tagGameDictionaries.Start();

            BiasTagPageRank();
        }

        private Dictionary<int, PRUserGame> GenerateUserGameDictionary()
        {
            Dictionary<int, PRUserGame> userGameDictionary = new Dictionary<int, PRUserGame>();
            foreach (UserGameTime.Game game in _userGames.Where(game => game.playtime_forever > 0))
            {
                if (CalculatePageRank.Games.ContainsKey(game.appid))
                {
                    PRUserGame userGame = new PRUserGame(CalculatePageRank.Games[game.appid], game);
                    userGameDictionary.Add(game.appid, userGame);
                    CalculatePageRank.Games.Remove(game.appid);
                }
            }
            return userGameDictionary;
        }

        private PRTagGameDictionaries GenerateTagGameDictionaries(Dictionary<int, PRUserGame> userGameDictionary)
        {
            Database db = new Database();
            Dictionary<int, Game> gameIDs =
                db.FindGamesById(userGameDictionary.Values.Select(item => item.AppID).ToList());
            PRTagGameDictionaries tagGameDictionaries = new PRTagGameDictionaries(gameIDs);

            return tagGameDictionaries;
        }

        private void BiasTagPageRank()
        {
            CalculateMultiplier();
            CalculatePageRank.UpdateGamePageRanks();
        }

        private void CalculateMultiplier()
        {
            int minimumOutlinks = _tagGameDictionaries.TagDictionary.Values.Min(tag => tag.OutLinks);
            int maximumOutlinks = _tagGameDictionaries.TagDictionary.Values.Max(tag => tag.OutLinks);
            int variance = maximumOutlinks - minimumOutlinks;
            double increment = 0.9/variance;
            double multiplier = 0.1;

            Dictionary<int, double> tagMultipliers = new Dictionary<int, double>();

            for (int outlinks = minimumOutlinks; outlinks <= maximumOutlinks; outlinks++)
            {
                tagMultipliers.Add(outlinks, multiplier += increment);
            }
            foreach (PRTag prTag in CalculatePageRank.Tags.Values)
            {
                if (_tagGameDictionaries.TagDictionary.ContainsKey(prTag.Tag))
                {
                    double tagMultiplier = tagMultipliers[_tagGameDictionaries.TagDictionary[prTag.Tag].OutLinks];
                    prTag.TagPageRank = prTag.TagPageRank*tagMultiplier;
                    continue;
                }
                prTag.TagPageRank = (double)1/CalculatePageRank.Games.Count;
            }
        }

        #endregion

    }
}