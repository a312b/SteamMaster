using System;

namespace PageRank
{
    /// <summary>
    /// This class contains the necessary information for tags for calculating their individual GamePageRank
    /// </summary>
    class PRTag : IComparable<PRTag>
    {
        public string Tag { get; private set; }
        public int TagIndex { get; private set; } //The index of the tag in tag vector
        public int InLinks { get; set; }
        public int OutLinks { get; set; } // Tag Frequency
        public double TagPageRank { get; set; }

        public PRTag(string tag, int index)
        {
            Tag = tag;
            TagIndex = index;
            OutLinks = 1;
            TagPageRank = 1; //Starting value, reconsider and / or argue this value
        }

        public int CompareTo(PRTag other)
        {
            return other.TagPageRank.CompareTo(this.TagPageRank);
        }
    }
}
