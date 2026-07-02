using System.Collections.Generic;
using System.Linq;

namespace PokerHandEvaluator.Models
{
    /// <summary>
    /// Represents a poker hand
    /// </summary>
    public class Hand
    {
        public List<Card> Cards { get; }

        public Hand(params Card[] cards)
        {
            Cards = new List<Card>(cards);
        }

        public Hand(params string[] cardNotations)
        {
            Cards = cardNotations.Select(Card.Parse).ToList();
        }

        public static Hand Parse(string notation)
        {
            var cards = new List<Card>();
            for (int i = 0; i < notation.Length; i += 2)
            {
                cards.Add(Card.Parse(notation.Substring(i, 2)));
            }
            return new Hand(cards.ToArray());
        }

        public override string ToString() => string.Concat(Cards);

        public int Compare(Hand handToCompare)
        {
            int compareCounter = 0;

            if (handToCompare.Cards.Contains(this.Cards[0]))
            {
                compareCounter++;
            }
            if (handToCompare.Cards.Contains(this.Cards[1]))
            {
                compareCounter++;
            }
            return compareCounter;
        }
    }
}
