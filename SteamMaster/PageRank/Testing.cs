using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using SteamSharpCore;

namespace PageRank
{
    class Testing
    {
        static void Main()
        {
            Database db = new Database();
            Dictionary<int, Game> games = db.FindAllGames();
            SteamSharp steamSharp = new SteamSharp("876246E23CF840EE87CB64FF6E4202E3");
            string Operdies = "76561198044471784";
            var userGames = steamSharp.SteamUserGameTimeListById(Operdies);
            PRGameRanker pageRank = new PRGameRanker(games, userGames);
            foreach (var game in pageRank.GetRankedGameList("hehe"))
            {
                Console.Write(game.Title);
                Console.CursorLeft = Console.WindowWidth/2;
                Console.WriteLine(game.GamePageRank);
            }
            Console.ReadKey();
        }
    }
}
