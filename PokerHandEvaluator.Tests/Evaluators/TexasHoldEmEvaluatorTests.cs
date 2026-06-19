using PokerHandEvaluator.Models;
using PokerHandEvaluator.Evaluators;
using Xunit;

namespace PokerHandEvaluator.Tests.Evaluators
{
    public class TexasHoldEmEvaluatorTests
    {
        private readonly TexasHoldEmEvaluator _evaluator = new();

        [Fact]
        public void Evaluate_RoyalFlush_ReturnsCorrectRanking()
        {
            var hand = Hand.Parse("AsKs");
            var community = new List<Card>
            {
                Card.Parse("QsJs"),
                Card.Parse("Ts"),
                Card.Parse("2c"),
                Card.Parse("3c")
            };

            var ranking = _evaluator.Evaluate(hand, community);

            Assert.Equal(HandRankType.RoyalFlush, ranking.Type);
        }

        [Fact]
        public void Evaluate_StraightFlush_ReturnsCorrectRanking()
        {
            var hand = Hand.Parse("9s8s");
            var community = new List<Card>
            {
                Card.Parse("7s"),
                Card.Parse("6s"),
                Card.Parse("5s"),
                Card.Parse("2c"),
                Card.Parse("3c")
            };

            var ranking = _evaluator.Evaluate(hand, community);

            Assert.Equal(HandRankType.StraightFlush, ranking.Type);
        }

        [Fact]
        public void Evaluate_FourOfAKind_ReturnsCorrectRanking()
        {
            var hand = Hand.Parse("AsAd");
            var community = new List<Card>
            {
                Card.Parse("Ah"),
                Card.Parse("Ac"),
                Card.Parse("2s"),
                Card.Parse("3d"),
                Card.Parse("4h")
            };

            var ranking = _evaluator.Evaluate(hand, community);

            Assert.Equal(HandRankType.FourOfAKind, ranking.Type);
        }

        [Fact]
        public void Evaluate_FullHouse_ReturnsCorrectRanking()
        {
            var hand = Hand.Parse("AsAd");
            var community = new List<Card>
            {
                Card.Parse("Ah"),
                Card.Parse("2c"),
                Card.Parse("2s"),
                Card.Parse("3d"),
                Card.Parse("4h")
            };

            var ranking = _evaluator.Evaluate(hand, community);

            Assert.Equal(HandRankType.FullHouse, ranking.Type);
        }

        [Fact]
        public void Evaluate_Flush_ReturnsCorrectRanking()
        {
            var hand = Hand.Parse("AsKs");
            var community = new List<Card>
            {
                Card.Parse("Qs"),
                Card.Parse("Js"),
                Card.Parse("9s"),
                Card.Parse("2c"),
                Card.Parse("3d")
            };

            var ranking = _evaluator.Evaluate(hand, community);

            Assert.Equal(HandRankType.Flush, ranking.Type);
        }

        [Fact]
        public void Evaluate_Straight_ReturnsCorrectRanking()
        {
            var hand = Hand.Parse("9s8h");
            var community = new List<Card>
            {
                Card.Parse("7d"),
                Card.Parse("6c"),
                Card.Parse("5s"),
                Card.Parse("2c"),
                Card.Parse("3d")
            };

            var ranking = _evaluator.Evaluate(hand, community);

            Assert.Equal(HandRankType.Straight, ranking.Type);
        }

        [Fact]
        public void Evaluate_Wheel_ReturnsCorrectRanking()
        {
            var hand = Hand.Parse("As2h");
            var community = new List<Card>
            {
                Card.Parse("3d"),
                Card.Parse("4c"),
                Card.Parse("5s"),
                Card.Parse("9c"),
                Card.Parse("Kd")
            };

            var ranking = _evaluator.Evaluate(hand, community);

            Assert.Equal(HandRankType.Straight, ranking.Type);
        }

        [Fact]
        public void Evaluate_ThreeOfAKind_ReturnsCorrectRanking()
        {
            var hand = Hand.Parse("AsAd");
            var community = new List<Card>
            {
                Card.Parse("Ah"),
                Card.Parse("2c"),
                Card.Parse("3s"),
                Card.Parse("4d"),
                Card.Parse("5h")
            };

            var ranking = _evaluator.Evaluate(hand, community);

            Assert.Equal(HandRankType.ThreeOfAKind, ranking.Type);
        }

        [Fact]
        public void Evaluate_TwoPair_ReturnsCorrectRanking()
        {
            var hand = Hand.Parse("AsAd");
            var community = new List<Card>
            {
                Card.Parse("2h"),
                Card.Parse("2c"),
                Card.Parse("3s"),
                Card.Parse("4d"),
                Card.Parse("5h")
            };

            var ranking = _evaluator.Evaluate(hand, community);

            Assert.Equal(HandRankType.TwoPair, ranking.Type);
        }

        [Fact]
        public void Evaluate_OnePair_ReturnsCorrectRanking()
        {
            var hand = Hand.Parse("AsAd");
            var community = new List<Card>
            {
                Card.Parse("2h"),
                Card.Parse("3c"),
                Card.Parse("4s"),
                Card.Parse("5d"),
                Card.Parse("6h")
            };

            var ranking = _evaluator.Evaluate(hand, community);

            Assert.Equal(HandRankType.OnePair, ranking.Type);
        }

        [Fact]
        public void Evaluate_HighCard_ReturnsCorrectRanking()
        {
            var hand = Hand.Parse("AsKd");
            var community = new List<Card>
            {
                Card.Parse("Qh"),
                Card.Parse("Jc"),
                Card.Parse("9s"),
                Card.Parse("2d"),
                Card.Parse("3h")
            };

            var ranking = _evaluator.Evaluate(hand, community);

            Assert.Equal(HandRankType.HighCard, ranking.Type);
        }

        [Fact]
        public void Evaluate_WrongHoleCardCount_ThrowsException()
        {
            var hand = Hand.Parse("AsAdAh");
            var community = new List<Card> { Card.Parse("2h"), Card.Parse("3c") };

            Assert.Throws<ArgumentException>(() => _evaluator.Evaluate(hand, community));
        }

        [Fact]
        public void Evaluate_ChoosesBestHand_WhenMultipleHandsExist()
        {
            // Hand has pair of Aces, but community has pair of Kings and a Queen
            var hand = Hand.Parse("AsAd");
            var community = new List<Card>
            {
                Card.Parse("Kh"),
                Card.Parse("Kc"),
                Card.Parse("Qs"),
                Card.Parse("2d"),
                Card.Parse("3h")
            };

            var ranking = _evaluator.Evaluate(hand, community);

            // Should pick pair of Aces over pair of Kings
            Assert.Equal(HandRankType.OnePair, ranking.Type);
            Assert.Equal(Rank.Ace, ranking.Ranks[0]);
        }
    }
}
