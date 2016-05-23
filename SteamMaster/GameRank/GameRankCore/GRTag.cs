using System;

namespace GameRank
{
    /// <summary>
    ///     This class contains the necessary information for tags for calculating their individual GameRank
    /// </summary>
    internal class GRTag : IComparable<GRTag>
    {
        #region Constructor

        public GRTag(string tag)
        {
            Tag = tag;
            Outlinks = 1;
            GameRank = 1; //Starting value, reconsider and / or argue this value
        }

        #endregion

        #region Methods

        public int CompareTo(GRTag other)
        {
            return other.GameRank.CompareTo(GameRank);
        }

        #endregion

        #region Fields

        public string Tag { get; }
        public int Outlinks { get; set; } // Tag Frequency
        public double GameRank { get; set; }

        #endregion
    }
}