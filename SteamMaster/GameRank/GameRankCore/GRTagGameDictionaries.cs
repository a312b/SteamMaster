using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DatabaseCore.lib.converter.models;

namespace GameRank
{
    /// <summary>
    ///     This class receives a dictionary of games and their associated app id's
    ///     and translates this into two separate dictionaries. One containing tags,
    ///     another containing games. The tags are assigned a unique index position
    ///     which decides their position in the tag vector that is assigned to each game.
    ///     The instance of this class is then passed to GRCalculateGameRank.
    /// </summary>
    class GRTagGameDictionaries
    {
        #region Constructor

        public GRTagGameDictionaries(Dictionary<int, Game> databaseGames)
        {
            DatabaseGames = databaseGames;
        }

        #endregion

        #region Fields

        private readonly Dictionary<int, Game> DatabaseGames;
        public Dictionary<string, GRTag> TagDictionary = new Dictionary<string, GRTag>();
        public Dictionary<int, GRGame> GameDictionary = new Dictionary<int, GRGame>();

        #endregion

        #region Methods

        public void Start()
        {
            InitializeTags();
            InitializeGames();
        }

        private void InitializeTags()
        {
            List<string> blacklistedTags = GetBlacklistedTags();
            //Iterates through all the tag lists in GameTagDictionary
            //foreach (string tag in GameTagDictionary.Values.SelectMany(gameTagList => gameTagList))
            foreach (Game game in DatabaseGames.Values)
            {
                foreach (string gameTag in game.Tags.Select(tag => tag.description))
                {
                    if (blacklistedTags.Contains(gameTag))
                        continue;
                    if (string.IsNullOrWhiteSpace(gameTag)) continue;

                    if (TagDictionary.ContainsKey(gameTag))
                        TagDictionary[gameTag].Outlinks++;
                    else if (!TagDictionary.ContainsKey(gameTag))
                        TagDictionary.Add(gameTag, new GRTag(gameTag));
                }
            }
        }

        /// <summary>
        ///     This function removes the tags that I have deemed redundant, either
        ///     because they don't really describe a game quality, or because
        ///     it is not related to gaming at all. This includes joke tags
        /// </summary>
        /// <param name="tagsAndGames"></param>
        /// <returns></returns>
        private List<string> GetBlacklistedTags()
        {
            List<string> blacklistedTags = new List<string>();
            string path = Directory.GetCurrentDirectory() + @"\Exclude list.txt";
            StreamReader reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line == null) break;
                blacklistedTags.Add(line);
            }
            reader.Close();
            return blacklistedTags;
        }

        private void InitializeGames()
        {
            //If a game contains at least one tag, genre or category, it is added to the game dictionary
            foreach (var game in DatabaseGames)
            {
                List<string> tagList = GetGenreTagsAndCategories(game.Value);
                if (tagList.Count > 0)
                    GameDictionary.Add(game.Value.SteamAppId,
                        new GRGame(game.Value.SteamAppId, tagList, game.Value.Title));
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

        #endregion
    }
}