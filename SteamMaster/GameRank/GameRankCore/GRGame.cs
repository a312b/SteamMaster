﻿using System;
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

        public GRGame(int appID, List<string> tags)
        {
            AppID = appID;
            Tags = tags;
            Outlinks = Tags.Count;
            
            GameRank = 1; //Starting value for GameRank
        }

        #endregion

        #region Methods

        public int CompareTo(GRGame other)
        {
            return other.GameRank.CompareTo(GameRank);
        }

        #endregion

        #region Fields

        public int AppID { get; }
        public List<string> Tags { get; }
        public int Outlinks { get; }
        //public Dictionary<string, int>  
        public double GameRank { get; set; }

        #endregion
    }
}