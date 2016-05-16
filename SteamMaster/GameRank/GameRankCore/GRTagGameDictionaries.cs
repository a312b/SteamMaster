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
            InitializeTagsAndGames();
        }

        /// <summary>
        /// Initializes the tag and game dictionaries
        /// </summary>
        private void InitializeTagsAndGames()
        {
            List<string> blacklistedTags = GetBlacklistedTags();

            foreach (Game game in DatabaseGames.Values)
            {
                List<string> tags = game.Tags.Select(tag => tag.description).ToList();
                List<string> gameTags = new List<string>();
                foreach (string tag in tags)
                {
                    if (blacklistedTags.Contains(tag)) continue;
                    if (string.IsNullOrWhiteSpace(tag)) continue;

                    if (TagDictionary.ContainsKey(tag))
                        TagDictionary[tag].Outlinks++;
                    else if (!TagDictionary.ContainsKey(tag))
                        TagDictionary.Add(tag, new GRTag(tag));

                    gameTags.Add(tag);
                }
                GameDictionary.Add(game.SteamAppId, new GRGame(game.SteamAppId, gameTags));
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
            DirectoryInfo fileFolder = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent;
            string path = fileFolder.FullName + @"\Exclude list.txt";

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

        #endregion
    }
}