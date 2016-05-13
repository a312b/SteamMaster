using System;

namespace GameRank
{
    /// <summary>
    /// This class contains the necessary information for tags for calculating their individual GameRank
    /// </summary>
    class GRTag : IComparable<GRTag>
    {
        public string Tag { get; private set; }
        public int TagIndex { get; private set; } //The index of the tag in tag vector
        public int InLinks { get; set; }
        public int OutLinks { get; set; } // Tag Frequency
        public double GameRank { get; set; }

        public GRTag(string tag, int index)
        {
            Tag = tag;
            TagIndex = index;
            OutLinks = 1;
            GameRank = 1; //Starting value, reconsider and / or argue this value
        }

        public int CompareTo(GRTag other)
        {
            return other.GameRank.CompareTo(this.GameRank);
        }
    }
}
