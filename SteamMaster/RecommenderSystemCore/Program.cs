using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatabaseCore;
using MongoDB.Bson.IO;
using PageRank;
using RecommenderSystemCore.Controller;
using SteamSharpCore;
using SteamSharpCore.steamStore;
using SteamSharpCore.steamStore.models;
using SteamUI;

namespace RecommenderSystemCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SteamTheme UI = new SteamTheme();
            RSController controller = new RSController(UI);
            Application.Run(UI); //StartSearchPage()
        }

    }
}
