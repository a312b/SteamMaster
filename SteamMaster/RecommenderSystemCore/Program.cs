using System.Windows.Forms;
using RecommenderSystemCore.Controller;
using SteamUI;

namespace RecommenderSystemCore
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SteamTheme UI = new SteamTheme();
            RSController controller = new RSController(UI);
            Application.Run(UI); //StartSearchPage()
        }
    }
}
