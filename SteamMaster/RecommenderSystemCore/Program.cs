using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RecommenderSystemCore.Controller;
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
