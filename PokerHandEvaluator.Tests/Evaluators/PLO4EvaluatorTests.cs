using PokerHandEvaluator.Models;
using PokerHandEvaluator.Evaluators;
using Xunit;
using System.Collections.Generic;
using System;

namespace PokerHandEvaluator.Tests.Evaluators
{
    public class PLO4EvaluatorTests
    {
        private readonly PLO4Evaluator _evaluator = new PLO4Evaluator();

        [Fact]
        public void Evaluate_ValidHand_ReturnsRanking()
        {
            var hand = Hand.Parse("AsKdQhJc");
            var community = new List<Card>
            {
                Card.Parse("2s"),
                Card.Parse("3h"),
                Card.Parse("4d")
            };

            var ranking = _evaluator.Evaluate(hand, community);

            Assert.NotNull(ranking);
        }

        [Fact]
        public void Evaluate_WrongHoleCardCount_ThrowsException()
        {
            var hand = Hand.Parse("AsKd");  // Only 2 cards, need 4
            var community = new List<Card>
            {
                Card.Parse("2s"),
                Card.Parse("3h"),
                Card.Parse("4d")
            };

            Assert.Throws<ArgumentException>(() => _evaluator.Evaluate(hand, community));
        }

        [Fact]
        public void Evaluate_InsufficientCommunityCards_ThrowsException()
        {
            var hand = Hand.Parse("AsKdQhJc");
            var community = new List<Card> { Card.Parse("2s") };  // Only 1 card, need at least 3

            Assert.Throws<ArgumentException>(() => _evaluator.Evaluate(hand, community));
        }

        [Fact]
        public void Evaluate_TooManyCommunityCards_ThrowsException()
        {
            var hand = Hand.Parse("AsKdQhJc");
            var community = new List<Card>
            {
                Card.Parse("2s"),
                Card.Parse("3h"),
                Card.Parse("4d"),
                Card.Parse("5c"),
                Card.Parse("6s"),
                Card.Parse("7h")  // 6 cards, max is 5
            };

            Assert.Throws<ArgumentException>(() => _evaluator.Evaluate(hand, community));
        }

        [Fact]
        public void Evaluate_Flop_ReturnsValidRanking()
        {
            var hand = Hand.Parse("AsKdQhJc");
            var flop = new List<Card>
            {
                Card.Parse("2s"),
                Card.Parse("3h"),
                Card.Parse("4d")
            };

            var ranking = _evaluator.Evaluate(hand, flop);

            Assert.NotNull(ranking);
        }

        [Fact]
        public void Evaluate_Turn_ReturnsValidRanking()
        {
            var hand = Hand.Parse("AsKdQhJc");
            var turn = new List<Card>
            {
                Card.Parse("2s"),
                Card.Parse("3h"),
                Card.Parse("4d"),
                Card.Parse("5c")
            };

            var ranking = _evaluator.Evaluate(hand, turn);

            Assert.NotNull(ranking);
        }

        [Fact]
        public void Evaluate_River_ReturnsValidRanking()
        {
            var hand = Hand.Parse("AsKdQhJc");
            var river = new List<Card>
            {
                Card.Parse("2s"),
                Card.Parse("3h"),
                Card.Parse("4d"),
                Card.Parse("5c"),
                Card.Parse("6h")
            };

            var ranking = _evaluator.Evaluate(hand, river);

            Assert.NotNull(ranking);
        }

        [Fact]
        public void Evaluate_ChoosesBestCombination_From2Plus3Rule()
        {
            // Hand: As Ks Ah 2h
            // Community: 3h 4h 5h
            // Best hand: Ks + 2h (from hand) with 3h + 4h + 5h (from community) = Flush
            var hand = Hand.Parse("AsKsAh2h");
            var community = new List<Card>
            {
                Card.Parse("3h"),
                Card.Parse("4h"),
                Card.Parse("5h")
            };

            var ranking = _evaluator.Evaluate(hand, community);

            // Should find a flush
            Assert.True(ranking.Type == HandRankType.Flush || ranking.Type == HandRankType.StraightFlush);
        }
    }
}
