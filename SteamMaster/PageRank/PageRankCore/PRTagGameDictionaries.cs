using System.Collections.Generic;
using System.Linq;
using DatabaseCore.lib.converter.models;
using SteamSharpCore.steamStore.models;

namespace PageRank
{
    /// <summary>
    /// This class receives a dictionary of games and their associated app id's
    /// and translates this into two separate dictionaries. One containing tags,
    /// another containing games. The tags are assigned a unique index position
    /// which decides their position in the tag vector that is assigned to each game.
    /// The instance of this class is then passed to PRCalculatePageRank.
    /// </summary>
    class PRTagGameDictionaries
    {
        #region Fields

        private readonly Dictionary<int, Game> DatabaseGames; 
        public Dictionary<string, PRTag> TagDictionary = new Dictionary<string, PRTag>();
        public Dictionary<int, PRGame> GameDictionary = new Dictionary<int, PRGame>();

        #endregion

        #region Constructor

        public PRTagGameDictionaries(Dictionary<int,Game> databaseGames)
        {
            DatabaseGames = databaseGames;
        }

        #endregion

        #region Methods
        public void Start()
        {
            InitializeTags();
            InitializeGames();
        }

        private void InitializeTags()
        {
            int tagIndexCounter = 0;
            //Iterates through all the tag lists in GameTagDictionary
            //foreach (string tag in GameTagDictionary.Values.SelectMany(gameTagList => gameTagList))
            foreach(Game game in DatabaseGames.Values)
            {
                foreach (SteamStoreGame.Tag gameTag in game.Tags)
                {
                    string tag = gameTag.description;
                    if (string.IsNullOrWhiteSpace(tag)) continue;

                    if (TagDictionary.ContainsKey(tag))
                        TagDictionary[tag].OutLinks++;
                    else if (!TagDictionary.ContainsKey(tag))
                        TagDictionary.Add(tag, new PRTag(tag, tagIndexCounter++));
                }
            }
            
        }

        private void InitializeGames()
        {
            //If a game contains at least one tag, genre or category, it is added to the game dictionary
            foreach (var game in DatabaseGames)
            {
                List<string> tagList = GetGenreTagsAndCategories(game.Value);
                int[] tagVector = GetTagVector(tagList);
                if (tagVector.Sum() != 0)
                    GameDictionary.Add(game.Value.SteamAppId, 
                        new PRGame(game.Value.SteamAppId, tagVector, tagList, game.Value.Title));
            }
        }

        //Tags, genres and categories are added to a collective list henceforth referred to as tags
        private List<string> GetGenreTagsAndCategories(Game game)
        {
            List<string> gameTags = new List<string>();
            if (game.Tags != null)
                gameTags.AddRange(game.Tags.Select(tag => tag.description));
            if (game.Categories != null)
                gameTags.AddRange(game.Categories.Select(category => category.description));
            if (game.Genres != null)
                gameTags.AddRange(game.Genres.Select(genre => genre.description));

            return gameTags;
        }

        //A tag vector is generated for each game according to its tags
        //For this purpose, genres and categories are treated the same as tags
        //Each tag has a unique index. The tag vector of this index will be either "1" or "0"
        private int[] GetTagVector(List<string> gameTagList)
        {
            int[] tagVector = new int[TagDictionary.Count];

            foreach (KeyValuePair<string, PRTag> tag in TagDictionary)
            {
                tagVector[tag.Value.TagIndex] = gameTagList.Contains(tag.Key) ? 1 : 0;
            }
            return tagVector;
        }
        #endregion
    }
}