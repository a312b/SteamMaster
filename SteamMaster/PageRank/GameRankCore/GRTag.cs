using System;

namespace GameRank
{
    /// <summary>
    /// This class contains the necessary information for tags for calculating their individual GameRank
    /// </summary>
    class GRTag : IComparable<GRTag>
    {
        #region Fields

        public string Tag { get; }
        public int TagIndex { get; } //The index of the tag in tag vector
        public int OutLinks { get; set; } // Tag Frequency
        public double GameRank { get; set; }

        #endregion


        #region Constructor

        public GRTag(string tag, int index)
        {
            Tag = tag;
            TagIndex = index;
            OutLinks = 1;
            GameRank = 1; //Starting value, reconsider and / or argue this value
        }

        #endregion


        #region Methods

        public int CompareTo(GRTag other)
        {
            return other.GameRank.CompareTo(this.GameRank);
        }

        #endregion

    }
}
