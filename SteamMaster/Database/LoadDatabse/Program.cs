using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using SteamSharpCore.steamSpy.models;
using SteamSharpCore.steamStore.models;
using Timer = System.Timers.Timer;

namespace LoadDatabse
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new Database.Database();
            var games = db.FindGameById("365590");
            foreach (var game in games)
            {
                Console.WriteLine(game.Title);
            }
            
            Console.ReadKey();
        }
    }
}
