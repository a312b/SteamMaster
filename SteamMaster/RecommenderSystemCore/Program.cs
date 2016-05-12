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
        [STAThread]
        static void Main(string[] args)
        {
            SteamTheme UI = new SteamTheme();
            RSController controller = new RSController(UI);
        }

    }
}
