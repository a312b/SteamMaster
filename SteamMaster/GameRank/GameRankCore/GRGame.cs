using System;
using System.Collections.Generic;

namespace GameRank
{
    /// <summary>
    ///     This class contains all the necessary information
    ///     about a game with regards to calculating GameRank
    /// </summary>
    public class GRGame : IComparable<GRGame>
    {
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

        #region Fields

        public string Title { get; }
        public int AppID { get; }
        public List<string> Tags { get; }
        public int Outlinks { get; }
        //public Dictionary<string, int>  
        public double GameRank { get; set; }

        #endregion

        #region Methods

        public int CompareTo(GRGame other)
        {
            return other.GameRank.CompareTo(this.GameRank);
        }

        #endregion
    }
}