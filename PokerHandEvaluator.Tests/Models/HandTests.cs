using PokerHandEvaluator.Models;
using Xunit;

namespace PokerHandEvaluator.Tests.Models
{
    public class HandTests
    {
        [Fact]
        public void Constructor_WithCards_CreatesHand()
        {
            var card1 = new Card(Rank.Ace, Suit.Spades);
            var card2 = new Card(Rank.King, Suit.Diamonds);
            var hand = new Hand(card1, card2);

            Assert.Equal(2, hand.Cards.Count);
            Assert.Contains(card1, hand.Cards);
            Assert.Contains(card2, hand.Cards);
        }

        [Fact]
        public void Constructor_WithNotations_CreatesHand()
        {
            var hand = new Hand("As", "Kd");

            Assert.Equal(2, hand.Cards.Count);
            Assert.Equal(Rank.Ace, hand.Cards[0].Rank);
            Assert.Equal(Rank.King, hand.Cards[1].Rank);
        }

        [Fact]
        public void Parse_ValidNotation_CreatesHand()
        {
            var hand = Hand.Parse("AsKdQhJc");

            Assert.Equal(4, hand.Cards.Count);
            Assert.Equal(Rank.Ace, hand.Cards[0].Rank);
            Assert.Equal(Rank.King, hand.Cards[1].Rank);
            Assert.Equal(Rank.Queen, hand.Cards[2].Rank);
            Assert.Equal(Rank.Jack, hand.Cards[3].Rank);
        }

        [Fact]
        public void ToString_ReturnsCorrectNotation()
        {
            var hand = new Hand("As", "Kd");
            Assert.Equal("AsKd", hand.ToString());
        }
    }
}
