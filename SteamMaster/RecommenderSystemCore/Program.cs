using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson.IO;
using RecommenderSystemCore.Controller;
using SteamSharp;
using SteamSharp.steamStore;
using SteamSharp.steamStore.models;
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
            RSController Control = new RSController(UI);

            Application.Run(UI); //StartSearchPage()
        }

    }
}
