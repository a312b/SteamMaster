using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using MongoDB.Driver.Core.Events;
using SteamSharpCore;
using SteamSharpCore.steamUser.models;

namespace PageRank
{
    public class PRGameRanker
    {
        #region Fields

        private List<UserGameTime.Game> _userGames;
        private Dictionary<int, Game> _databaseGames;
        private List<PRGame> _userRecommendations;

        #endregion

        #region Constructor

        public PRGameRanker(Dictionary<int, Game> databaseGames, List<UserGameTime.Game> userGames)
        {
            _databaseGames = databaseGames;
            _userGames = userGames;
        }

        #endregion

        #region Methods

        public List<PRGame> GetRankedGameList()
        {
            Start();
            return _userRecommendations.Take(30).ToList();
            //return _userRecommendations.Select(game => game.AppID).Take(30).ToList();
        }

        private Dictionary<int, Game> RemoveDemoGames(Dictionary<int, Game> gameDictionary)
        {
            List<int> demoGames = new List<int>();
            foreach (Game game in gameDictionary.Values)
            {
                string[] segments = game.Title.Split(' ');
                foreach (string segment in segments)
                {
                    if (segment.ToLower().Equals("demo"))
                        demoGames.Add(game.SteamAppId);
                }
            }
            foreach (int id in demoGames)
            {
                gameDictionary.Remove(id);
            }
            return gameDictionary;
        }

        private void Start()
        {
            _databaseGames = RemoveDemoGames(_databaseGames);
            PRTagGameDictionaries tagsAndGames = new PRTagGameDictionaries(_databaseGames);
            tagsAndGames.Start();
            try
            {
                tagsAndGames = RemoveRedundantTags(tagsAndGames);
            }
            catch (Exception)
            {
                //Place the exlude list textfile in your documents folder
            }
            PRCalculatePageRank unbiasedPageRank = new PRCalculatePageRank(tagsAndGames);
            unbiasedPageRank.Start();
            var userPageRank = new PRCalculatePersonalizedPageRank(unbiasedPageRank, _userGames);
            userPageRank.Start();
            _userRecommendations = new List<PRGame>(userPageRank.CalculatePageRank.Games.Values.ToList());
            _userRecommendations.Sort();
        }

        private PRTagGameDictionaries RemoveRedundantTags(PRTagGameDictionaries tagsAndGames)
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