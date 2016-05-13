﻿using System;
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
    /// <summary>
    /// Serves as an interface for calculating a ranked list 
    /// of game recommendation based on input games.
    /// </summary>
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

        
        public List<Game> GetRankedGameList()
        {
            Start();

            List<Game> recommendations = _userRecommendations.Select(game => _databaseGames[game.AppID]).ToList();
            return recommendations;
        }

        public List<PRGame> GetRankedGameList(string lol)
        {
            //This is a test class for getting actual pagerank scores
            Start();
            return _userRecommendations.Take(50).ToList();
        }




        private void Start()
        {
            _databaseGames = RemoveDemoGames(_databaseGames);

            PRTagGameDictionaries tagsAndGames = new PRTagGameDictionaries(_databaseGames);
            tagsAndGames.Start();
            tagsAndGames = RemoveRedundantTags(tagsAndGames);

            PRCalculatePageRank unbiasedPageRank = new PRCalculatePageRank(tagsAndGames, 0.25,0.0001, 10);
            unbiasedPageRank.Start();

            var userPageRank = new PRCalculatePersonalizedPageRank(unbiasedPageRank, _userGames);
            userPageRank.Start();

            _userRecommendations = new List<PRGame>(userPageRank.CalculatePageRank.Games.Values.ToList());
            _userRecommendations.Sort();
        }

        /// <summary>
        /// This function removes demo games. Not because they are irrelevant, but because
        /// demo games usually have the same tags as the actual released game, and therefore
        /// will have the same ranking. And we would rather recommend a released game than 
        /// a game demo.
        /// </summary>
        /// <param name="gameDictionary"></param>
        /// <returns></returns>
        private Dictionary<int, Game> RemoveDemoGames(Dictionary<int, Game> gameDictionary)
        {
            List<int> demoGames = new List<int>();
            //The linq though
            foreach (Game game in gameDictionary.Values)
            {
                string[] segments = game.Title.Split(' ');
                foreach (string segment in segments)
                {
                    if (segment.ToLower().Contains("sdk"))
                    {
                        demoGames.Add(game.SteamAppId);
                        continue;
                    }
                    if (segment.ToLower().Contains("dlc"))
                    {
                        demoGames.Add(game.SteamAppId);
                        continue;
                    }
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
        /// <summary>
        /// This function removes the tags that I have deemed redundant, either
        /// because they don't really describe a game quality, or because
        /// it is not related to gaming at all. This includes joke tags
        /// </summary>
        /// <param name="tagsAndGames"></param>
        /// <returns></returns>
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