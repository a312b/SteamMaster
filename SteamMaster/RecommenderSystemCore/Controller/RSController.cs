using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SteamSharp.steamStore.models;
using SteamUI;

namespace RecommenderSystemCore.Controller
{
    class RSController
    {
        public RSController(SteamTheme ui)
        {
            ui.RecommendButtomClick += RecommendGameList;

            UI = ui;
        }


        public SteamTheme UI { get; set; }

        private void RecommendGameList(string steamID) // fix senere
        {
            SteamSharp.SteamSharp _steamSharpTest = new SteamSharp.SteamSharp();
            int roundCount = 0;
            string steamId = UI.steamIdTextBox.Text.ToLower(); //not used
            string[] idArray =
            {
                "340", "280", "570", "80", "240", "400", "343780", "500", "374320", "10500", "252950", "300", "7940",
                "10180"
            };
            //150, 22380, 377160 == nullreference på htmlagility 
            //340, 570 == nullreference på prisen (der findes f.eks. "price_in_cents_with_discount" i stedet)

            List<SteamStoreGame> formGameList = _steamSharpTest.GameListByIds(idArray);
            if (formGameList != null)
            {
                UI.ClearGameListBox();
                foreach (SteamStoreGame game in formGameList)
                {
                    //ThreadPool.QueueUserWorkItem(LoadHeaderImages(game.data.steam_appid, roundCount));
                    UI.LoadHeaderImages(game.data.steam_appid, roundCount);
                    UI.LoadGameInfo(game, roundCount);
                    roundCount++;
                }
            }
        }


    }
}
