using PokerHandEvaluator.Models;
using System;
using Xunit;

namespace PokerHandEvaluator.Tests.Models
{
    public class CardTests
    {
        [Theory]
        [InlineData("As", Rank.Ace, Suit.Spades)]
        [InlineData("Kd", Rank.King, Suit.Diamonds)]
        [InlineData("Qh", Rank.Queen, Suit.Hearts)]
        [InlineData("Jc", Rank.Jack, Suit.Clubs)]
        [InlineData("Ts", Rank.Ten, Suit.Spades)]
        [InlineData("9h", Rank.Nine, Suit.Hearts)]
        [InlineData("2c", Rank.Two, Suit.Clubs)]
        public void Parse_ValidNotation_ReturnsCorrectCard(string notation, Rank expectedRank, Suit expectedSuit)
        {
            var card = Card.Parse(notation);

            Assert.Equal(expectedRank, card.Rank);
            Assert.Equal(expectedSuit, card.Suit);
        }

        [Theory]
        [InlineData("X")]
        [InlineData("AX")]
        [InlineData("XA")]
        [InlineData("ABC")]
        [InlineData("")]
        public void Parse_InvalidNotation_ThrowsArgumentException(string notation)
        {
            Assert.Throws<ArgumentException>(() => Card.Parse(notation));
        }

        [Fact]
        public void ToString_ReturnsCorrectNotation()
        {
            var card = new Card(Rank.Ace, Suit.Spades);
            Assert.Equal("As", card.ToString());
        }

        [Fact]
        public void Equals_SameCard_ReturnsTrue()
        {
            var card1 = new Card(Rank.Ace, Suit.Spades);
            var card2 = new Card(Rank.Ace, Suit.Spades);

            Assert.Equal(card1, card2);
        }

        [Fact]
        public void Equals_DifferentCard_ReturnsFalse()
        {
            var card1 = new Card(Rank.Ace, Suit.Spades);
            var card2 = new Card(Rank.King, Suit.Spades);

            Assert.NotEqual(card1, card2);
        }

        [Fact]
        public void GetHashCode_SameCard_ReturnsSameHash()
        {
            var card1 = new Card(Rank.Ace, Suit.Spades);
            var card2 = new Card(Rank.Ace, Suit.Spades);

            Assert.Equal(card1.GetHashCode(), card2.GetHashCode());
        }
    }
}
