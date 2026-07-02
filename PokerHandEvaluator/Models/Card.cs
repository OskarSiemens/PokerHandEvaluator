using System;

namespace PokerHandEvaluator.Models
{
    /// <summary>
    /// Represents a playing card with rank and suit
    /// </summary>
    public class Card
    {
        public Rank Rank { get; }
        public Suit Suit { get; }

        public Card(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public static Card Parse(string notation)
        {
            if (notation.Length != 2)
                throw new ArgumentException("Card notation must be 2 characters (e.g., 'AsKd')");

            var rank = notation[0] switch
            {
                'A' => Rank.Ace,
                'K' => Rank.King,
                'Q' => Rank.Queen,
                'J' => Rank.Jack,
                'T' => Rank.Ten,
                '9' => Rank.Nine,
                '8' => Rank.Eight,
                '7' => Rank.Seven,
                '6' => Rank.Six,
                '5' => Rank.Five,
                '4' => Rank.Four,
                '3' => Rank.Three,
                '2' => Rank.Two,
                _ => throw new ArgumentException($"Invalid rank: {notation[0]}")
            };

            var suit = notation[1] switch
            {
                's' => Suit.Spades,
                'h' => Suit.Hearts,
                'd' => Suit.Diamonds,
                'c' => Suit.Clubs,
                _ => throw new ArgumentException($"Invalid suit: {notation[1]}")
            };

            return new Card(rank, suit);
        }

        public override string ToString() => $"{RankToString(Rank)}{SuitToString(Suit)}";

        private static string RankToString(Rank rank) => rank switch
        {
            Rank.Ace => "A",
            Rank.King => "K",
            Rank.Queen => "Q",
            Rank.Jack => "J",
            Rank.Ten => "T",
            Rank.Nine => "9",
            Rank.Eight => "8",
            Rank.Seven => "7",
            Rank.Six => "6",
            Rank.Five => "5",
            Rank.Four => "4",
            Rank.Three => "3",
            Rank.Two => "2",
            _ => ""
        };

        private static string SuitToString(Suit suit) => suit switch
        {
            Suit.Spades => "s",
            Suit.Hearts => "h",
            Suit.Diamonds => "d",
            Suit.Clubs => "c",
            _ => ""
        };

        public override bool Equals(object? obj) =>
            obj is Card card && card.Rank == Rank && card.Suit == Suit;

        public override int GetHashCode() => HashCode.Combine(Rank, Suit);
    }

    public enum Rank
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14
    }

    public enum Suit
    {
        Spades,
        Hearts,
        Diamonds,
        Clubs
    }
}
