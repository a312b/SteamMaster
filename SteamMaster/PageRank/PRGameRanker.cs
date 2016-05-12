using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using MongoDB.Driver.Core.Events;
using SteamSharpCore;

namespace PageRank
{
    class PRGameRanker
    {
        #region Fields

        private SteamSharp _steamSharp;
        private Database _db;
        private List<PRGame> _userRecommendations; 

        public string SteamUserID { get; }

        #endregion

        #region Constructor

        public PRGameRanker(SteamSharp steamSharp, Database database, string SteamUser)
        {
            _steamSharp = steamSharp;
            _db = database;
            SteamUserID = SteamUser;

        }

        #endregion
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
            Dictionary<int, Game> allGames = _db.FindAllGames();
            allGames = RemoveDemoGames(allGames);
            PRTagGameDictionaries tagsAndGames = new PRTagGameDictionaries(allGames);
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
            PRCalculatePersonalizedPageRank userPageRank = new PRCalculatePersonalizedPageRank(unbiasedPageRank, SteamUserID, _steamSharp);
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


        static void Main(string[] args)
        {
            string apikey = "876246E23CF840EE87CB64FF6E4202E3";

            PRGameRanker PRGameRanker = new PRGameRanker(new SteamSharp(apikey), new Database(), "76561198044471784");
            var recommendations = PRGameRanker.GetRankedGameList();
            foreach (var recommendation in recommendations)
            {
                Console.WriteLine(recommendation.Title + "  " + recommendation.GamePageRank);
            }
            Console.ReadKey();
            /*
            const string APIKey = "876246E23CF840EE87CB64FF6E4202E3"; //Use your own plz
            //string steamUserId = "76561198044471784"; //Operdies
            //string Operdies64 = "76561197987505654";//DKYde
            //string Operdies64 = "76561198019106142"; //Jakob

            Database db = new Database();
            Dictionary <int, Game> allGames = db.FindAllGames();
            
            PRTagGameDictionaries tagsAndGames = new PRTagGameDictionaries(allGames);
            tagsAndGames.Start();

            PRCalculatePageRank calculatePageRank = new PRCalculatePageRank(tagsAndGames, 0.25D, 0.001D, 10);
            calculatePageRank.Start();
            List<PRTag> unbiasedTags = calculatePageRank.Tags.Values.ToList();
            unbiasedTags.Sort();

            List<PRGame> preUserRankedGames = new List<PRGame>(calculatePageRank.Games.Values.ToList());
            preUserRankedGames.Sort();
            PRCalculatePersonalizedPageRank PPR = new PRCalculatePersonalizedPageRank(calculatePageRank, steamUserId, APIKey);
            PPR.Start();
            List<PRTag> userTags = new List<PRTag>(PPR.CalculatePageRank.Tags.Values.ToList());
            userTags.Sort();

            List<PRGame> finalPageRankedGames = new List<PRGame>(PPR.CalculatePageRank.Games.Values);
            finalPageRankedGames.Sort();



            List<int> idList = new List<int>(finalPageRankedGames.Select(item => item.AppID).ToList().Take(30));
            //idList er top nice og sorteret efter spillenes pagerank score

            List<Game> dbGameListSortedByPageRank = idList.Select(id => allGames[id]).ToList();
            //Du kan eventuelt overveje at snuppe database spil direkte


            for (int i = 0; i < 30; i++)
            {
                finalPageRankedGames[i].PrintGame();
                //Console.WriteLine(finalPageRankedGames[i].AppID.ToString() + "   equals   " + idList[i].ToString());
                //Testede lige for en sikkerheds skyld at ID listen var i den rigtige rækkefølge
            }
            
            //List<UserGameTime.Game> userLibrary = steamSharp.SteamUserGameTimeListById(Operdies64);
            
            //Dictionary<int, PRUserGame> userGameDictionary = new Dictionary<int, PRUserGame>();
            //foreach (var libraryGame in userLibrary)
            //{
            //    if (calculatePageRank.Games.ContainsKey(libraryGame.appid))
            //    {
            //        PRUserGame userGame = new PRUserGame(calculatePageRank.Games[libraryGame.appid], libraryGame);
            //        userGameDictionary.Add(libraryGame.appid, userGame);
            //    }
            //}
            //Dictionary<int, Game> gameList = db.FindGamesById(userGameDictionary.Values.Select(game => game.AppID).ToList());
            
            //List<PRTag> tagList = calculatePageRank.Tags.Values.ToList();


            //tagList.Sort();

            //foreach (var tag in tagList)
            //{
            //    Console.WriteLine($"{tag.Tag}---------{tag.TagPageRank}");
            //}

            //Console.WriteLine("done, bruh");
            Console.ReadKey();
            */
        }
    }
}