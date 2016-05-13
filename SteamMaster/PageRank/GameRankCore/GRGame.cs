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
        public int AppID { get; }
        public List<string> Tags { get; }
        public int Outlinks { get; }
        //public Dictionary<string, int>  
        public double GameRank { get; set; }

        #endregion

        #region Constructor

        public GRGame(int appID, List<string> tags, string title)
        {
            AppID = appID;
            Tags = tags;
            Outlinks = Tags.Count;
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
