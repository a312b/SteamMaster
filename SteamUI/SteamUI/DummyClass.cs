using System.Collections.Generic;

//NOTICE:
//No longer the used data class, but kept alive for legacy code
//!
namespace SteamUI
{
    internal class DummyClass
    {
        private readonly List<Game> GameList = new List<Game>();
        private readonly List<string> NameList = new List<string>();

        public DummyClass() //i appid, s name, s dev, i releae, s desc, params
        {
            var CSCZ = new Game(19.99, 80, "Counter-Strike: Condition Zero", "Valve", 2004,
                "With its extensive Tour of Duty campaign, a near-limitless number of skirmish modes, updates and new content for Counter-Strike's award-winning multiplayer game play, plus over 12 bonus single player missions, Counter-Strike: Condition Zero is a tremendous offering of single and multiplayer content. ",
                "MMORPG", "Action", "Fantasy", "Multiplayer", "Free To Play");
            var CSCZBeta = new Game(19.99, 150, "Counter-Strike: CZ Beta", "Valve", 2004, "Description of the game",
                "Action", "Animal", "Singleplayer", "Open World", "Sandbox");
            var CSS = new Game(19.99, 240, "Counter-Strike: Source", "Valve", 2004, "Description of the game", "Horror",
                "Funny", "Fantasy", "Co-op", "Sandbox", "Moddability");
            var HLS = new Game(19.99, 280, "Half-Life: Source", "Valve", 2004, "Description of the game", "Funny",
                "Dark", "Singleplayer", "Free To Play");
            var HL2LC = new Game(19.99, 340, "Half-Life 2: Lost Coast", "Valve", 2004, "Description of the game", "Gore",
                "Horror", "Dark", "Action", "Singleplayer", "Multiplayer", "Co-op", "Open World");
            var POR = new Game(19.99, 400, "Portal", "Valve", 2004, "Description of the game", "Space", "Action",
                "Multiplayer", "Moddability");
            var TF2 = new Game(19.99, 440, "Team Fortress 2", "Valve", 2004, "Description of the game", "Space",
                "Animal", "Multiplayer", "Moddability");
            var L4D = new Game(19.99, 500, "Left 4 Dead", "Valve", 2004, "Description of the game", "Animal", "Dark",
                "Singleplayer", "Free To Play");
            var DOTA = new Game(19.99, 570, "Dota 2", "Valve", 2004, "Description of the game", "Boring", "MMORPG",
                "Multiplayer", "Sandbox", "Open World");
            var DODS = new Game(19.99, 300, "Day of Defeat: Source", "Valve", 2004, "Description of the game", "Funny",
                "Space", "Fantasy", "Action", "Singleplayer", "Co-op", "Moddability");
            var COD4 = new Game(19.99, 7940, "Call of Duty 4: MW", "Valve", 2004, "Description of the game", "Shooter",
                "Action", "Multiplayer", "Moddability", "Co-op");
            var CODMW2 = new Game(19.99, 10180, "Call of Duty: MW 2", "Valve", 2004, "Description of the game", "Space",
                "Action", "Multiplayer", "Moddability");
            var FLLNW = new Game(19.99, 22380, "Fallout: New Vegas", "Valve", 2004, "Description of the game", "Space",
                "Fantasy", "Action", "Singleplayer", "Moddability");
            var FLL4 = new Game(19.99, 377160, "Fallout 4", "Valve", 2004, "Description of the game", "Space", "Action",
                "Singleplayer");

            GameList.Add(CSCZ);
            GameList.Add(CSCZBeta);
            GameList.Add(CSS);
            GameList.Add(HLS);
            GameList.Add(HL2LC);
            GameList.Add(POR);
            GameList.Add(TF2);
            GameList.Add(L4D);
            GameList.Add(DOTA);
            GameList.Add(DODS);
            GameList.Add(COD4);
            GameList.Add(CODMW2);
            GameList.Add(FLLNW);
            GameList.Add(FLL4);

            NameList.Add("gustav");
            NameList.Add("julius");
            NameList.Add("ronja");
            NameList.Add("aleksander");
            NameList.Add("petter");
            NameList.Add("peter");
            NameList.Add("jeppe");
            NameList.Add("jacob");
            NameList.Add("niclas");
            NameList.Add("pubaaah");
            NameList.Add("aleqsander");
            NameList.Add("aleqxander");
            NameList.Add("morty");
            NameList.Add("acceptableuser");
            NameList.Add("user");
            NameList.Add("username");
        }

        public List<Game> GetGameListByName(string SteamID)
        {
            if (IDCompare(SteamID))
            {
                var steamID = SteamID;
                return GameList;
            }
            return null;
        }

        public List<Game> GetList()
        {
            return GameList;
        }

        private bool IDCompare(string ID)
        {
            var result = false;

            foreach (var item in NameList)
            {
                var match = string.Compare(ID, item) == 0;
                result |= match; // Mindst en compare returnerer true
            }

            return result;
        }
    }


    public class Game
    {
        private static int ID;

        public Game(double _price, int _appId, string _name, string _dev, int _releaseYear, string _description,
            params string[] _genre)
        {
            Price = _price;
            Name = _name;
            Genre = _genre;
            GameID = ID++;
            AppId = _appId;
            Developer = _dev;
            ReleaseYear = _releaseYear;
            Description = _description;
        }

        public int AppId { get; private set; }
        public string Name { get; private set; }
        public int GameID { get; private set; }
        public string Developer { get; set; }
        public int ReleaseYear { get; set; }
        public string Description { get; set; }
        public string[] Genre { get; private set; }
        public string[] Tags { get; private set; }
        public double Price { get; private set; }
    }
}