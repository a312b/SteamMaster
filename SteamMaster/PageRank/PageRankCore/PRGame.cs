using System;
using System.Collections.Generic;
using System.Linq;

namespace PageRank
{
    public class PRGame : IComparable<PRGame>
    {
        #region Fields

        public string Title { get; }

        public int AppID { get; private set; }
        public int[] TagVector { get; }
        public List<string> GameTags { get; private set; }
        public int OutgoingLinks { get; private set; }
        //public Dictionary<string, int>  
        public double GamePageRank { get; set; }

        #endregion

        public PRGame(int appID, int[] tagVector, List<string> gameTags, string title)
        {
            AppID = appID;
            TagVector = tagVector;
            OutgoingLinks = TagVector.Sum();
            GameTags = gameTags;
            Title = title;
            GamePageRank = 1; //Starting value for GamePageRank
            //GetInitialValue();

        }

        //private double GetInitialValue()
        //{
        //    return OutgoingLinks < 10 ? 1 : 2;
        //}
        public int CompareTo(PRGame other)
        {
            return other.GamePageRank.CompareTo(this.GamePageRank);
            //return other.GamePageRank.CompareTo(this.GamePageRank);
        }

        public override string ToString()
        {
            return $"{Title} --- {GamePageRank}";
        }

        public void PrintGame()
        {
            Console.Write(Title);
            Console.CursorLeft = Console.WindowWidth/2;
            Console.WriteLine(GamePageRank);
        }
    }
}
