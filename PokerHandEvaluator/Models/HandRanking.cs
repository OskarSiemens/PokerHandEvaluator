using System.Collections.Generic;

namespace PokerHandEvaluator.Models
{
    /// <summary>
    /// Represents a ranked poker hand
    /// </summary>
    public class HandRanking
    {
        public HandRankType Type { get; set; }
        public List<Rank> Ranks { get; set; }

        public HandRanking(HandRankType type, List<Rank> ranks)
        {
            Type = type;
            Ranks = ranks;
        }

        public int CompareTo(HandRanking other)
        {
            if (Type != other.Type)
                return other.Type.CompareTo(Type);

            for (int i = 0; i < Ranks.Count && i < other.Ranks.Count; i++)
            {
                if (Ranks[i] != other.Ranks[i])
                    return other.Ranks[i].CompareTo(Ranks[i]);
            }

            return 0;
        }

        public override string ToString() => $"{Type} - {string.Join(", ", Ranks)}";
    }

    public enum HandRankType
    {
        HighCard = 0,
        OnePair = 1,
        TwoPair = 2,
        ThreeOfAKind = 3,
        Straight = 4,
        Flush = 5,
        FullHouse = 6,
        FourOfAKind = 7,
        StraightFlush = 8,
        RoyalFlush = 9
    }
}
