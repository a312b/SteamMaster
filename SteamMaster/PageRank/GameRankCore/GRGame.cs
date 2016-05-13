using System;
using System.Collections.Generic;
using System.Linq;

namespace GameRank
{
    /// <summary>
    /// This class contains all the necessary information
    /// about a game with regards to calculating GameRank
    /// </summary>
    public class GRGame : IComparable<GRGame>
    {
        #region Fields

        public string Title { get; }

        public int AppID { get; private set; }
        public int[] TagVector { get; }
        public List<string> GameTags { get; private set; }
        public int OutgoingLinks { get; private set; }
        //public Dictionary<string, int>  
        public double GameRank { get; set; }

        #endregion

        #region Constructor

        public GRGame(int appID, int[] tagVector, List<string> gameTags, string title)
        {
            AppID = appID;
            TagVector = tagVector;
            OutgoingLinks = TagVector.Sum();
            GameTags = gameTags;
            Title = title;
            GameRank = 1; //Starting value for GameRank
        }

        #endregion


        #region Methods

        public int CompareTo(GRGame other)
        {
            return other.GameRank.CompareTo(this.GameRank);
            //return other.GameRank.CompareTo(this.GameRank);
        }

        public override string ToString()
        {
            return $"{Title} --- {GameRank}";
        }

        public void PrintGame()
        {
            Console.Write(Title);
            Console.CursorLeft = Console.WindowWidth/2;
            Console.WriteLine(GameRank);
        }

        #endregion

    }
}
