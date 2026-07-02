using PokerHandEvaluator.Models;
using PokerHandEvaluator.Simulators;
using System;
using System.Collections.Generic;
using Xunit;

namespace PokerHandEvaluator.Tests.Simulators
{
    public class HandSimulatorTests
    {
        [Fact]
        public void Simulate_TexasHoldEm_PreFlop_ReturnsValidResult()
        {
            var simulator = new HandSimulator(GameType.TexasHoldEm);
            var playerA = Hand.Parse("AsAd");
            var playerB = Hand.Parse("2h2c");
            var communityCards = new List<Card>();

            var result = simulator.Simulate(playerA, playerB, communityCards, 1000);

            Assert.Equal(1000, result.TotalSimulations);
            Assert.True(result.PlayerAWins + result.PlayerBWins + result.Ties == 1000);
            Assert.True(result.PlayerAWins > 0);  // AA should beat 22 more often
        }

        [Fact]
        public void Simulate_TexasHoldEm_WithFlop_ReturnsValidResult()
        {
            var simulator = new HandSimulator(GameType.TexasHoldEm);
            var playerA = Hand.Parse("AsKd");
            var playerB = Hand.Parse("2h3c");
            var flop = new List<Card>
            {
                Card.Parse("As"),
                Card.Parse("Ks"),
                Card.Parse("2s")
            };

            var result = simulator.Simulate(playerA, playerB, flop, 1000);

            Assert.Equal(1000, result.TotalSimulations);
            Assert.True(result.PlayerAWins + result.PlayerBWins + result.Ties == 1000);
        }

        [Fact]
        public void Simulate_TexasHoldEm_WithTurn_ReturnsValidResult()
        {
            var simulator = new HandSimulator(GameType.TexasHoldEm);
            var playerA = Hand.Parse("AsKd");
            var playerB = Hand.Parse("QhQc");
            var turn = new List<Card>
            {
                Card.Parse("2s"),
                Card.Parse("3h"),
                Card.Parse("4d"),
                Card.Parse("5c")
            };

            var result = simulator.Simulate(playerA, playerB, turn, 1000);

            Assert.Equal(1000, result.TotalSimulations);
            Assert.True(result.PlayerAWins + result.PlayerBWins + result.Ties == 1000);
        }

        [Fact]
        public void Simulate_TexasHoldEm_WithRiver_ReturnsValidResult()
        {
            var simulator = new HandSimulator(GameType.TexasHoldEm);
            var playerA = Hand.Parse("AsKd");
            var playerB = Hand.Parse("QhQc");
            var river = new List<Card>
            {
                Card.Parse("2s"),
                Card.Parse("3h"),
                Card.Parse("4d"),
                Card.Parse("5c"),
                Card.Parse("6s")
            };

            var result = simulator.Simulate(playerA, playerB, river, 1000);

            Assert.Equal(1000, result.TotalSimulations);
            Assert.True(result.PlayerAWins + result.PlayerBWins + result.Ties == 1000);
        }

        [Fact]
        public void Simulate_PLO4_PreFlop_ReturnsValidResult()
        {
            var simulator = new HandSimulator(GameType.PLO4);
            var playerA = Hand.Parse("AsAdAhAc");
            var playerB = Hand.Parse("2h2c2d2s");
            var communityCards = new List<Card>();

            var result = simulator.Simulate(playerA, playerB, communityCards, 100);

            Assert.Equal(100, result.TotalSimulations);
            Assert.True(result.PlayerAWins + result.PlayerBWins + result.Ties == 100);
        }

        [Fact]
        public void Simulate_PLO4_WithFlop_ReturnsValidResult()
        {
            var simulator = new HandSimulator(GameType.PLO4);
            var playerA = Hand.Parse("AsKsQsJs");
            var playerB = Hand.Parse("2h3h4h5h");
            var flop = new List<Card>
            {
                Card.Parse("Ts"),
                Card.Parse("9s"),
                Card.Parse("8s")
            };

            var result = simulator.Simulate(playerA, playerB, flop, 100);

            Assert.Equal(100, result.TotalSimulations);
            Assert.True(result.PlayerAWins + result.PlayerBWins + result.Ties == 100);
        }

        [Fact]
        public void Simulate_ZeroSimulations_ThrowsException()
        {
            var simulator = new HandSimulator(GameType.TexasHoldEm);
            var playerA = Hand.Parse("AsAd");
            var playerB = Hand.Parse("2h2c");

            Assert.Throws<ArgumentException>(() => 
                simulator.Simulate(playerA, playerB, new List<Card>(), 0));
        }

        [Fact]
        public void Simulate_NegativeSimulations_ThrowsException()
        {
            var simulator = new HandSimulator(GameType.TexasHoldEm);
            var playerA = Hand.Parse("AsAd");
            var playerB = Hand.Parse("2h2c");

            Assert.Throws<ArgumentException>(() => 
                simulator.Simulate(playerA, playerB, new List<Card>(), -100));
        }

        [Fact]
        public void Simulate_LargeNumberOfSimulations_CompletesSuccessfully()
        {
            var simulator = new HandSimulator(GameType.TexasHoldEm);
            var playerA = Hand.Parse("AsAd");
            var playerB = Hand.Parse("2h2c");

            var result = simulator.Simulate(playerA, playerB, new List<Card>(), 10000);

            Assert.Equal(10000, result.TotalSimulations);
            Assert.True(result.PlayerAWins > result.PlayerBWins);  // AA should win significantly
        }

        [Fact]
        public void Simulate_IdenticalHands_ProducesHighTieRate()
        {
            var simulator = new HandSimulator(GameType.TexasHoldEm);
            var playerA = Hand.Parse("AsAd");
            var playerB = Hand.Parse("AcAh");
            var communityCards = new List<Card>();

            var result = simulator.Simulate(playerA, playerB, communityCards, 100000);

            // With identical hands, should have mostly ties
            Assert.True(result.Ties > result.PlayerAWins + result.PlayerBWins);
        }

        [Fact]
        public void Simulate_IdenticalHandsWithCommunityCards_ProducesHighTieRate()
        {
            var simulator = new HandSimulator(GameType.TexasHoldEm);
            var playerA = Hand.Parse("AsAd");
            var playerB = Hand.Parse("AsAd");
            var communityCards = new List<Card>() { Card.Parse("Ts"), Card.Parse("Ks"), Card.Parse("Qs") };

            var result = simulator.Simulate(playerA, playerB, communityCards, 1000);

            // With identical hands, should have mostly ties
            Assert.True(result.Ties > result.PlayerAWins + result.PlayerBWins);
        }

        [Fact]
        public void Simulate_IdenticalHandsAkAk_ProducesHighTieRate()
        {
            var simulator = new HandSimulator(GameType.TexasHoldEm);
            var playerA = Hand.Parse("AsKs");
            var playerB = Hand.Parse("AdAd");
            var communityCards = new List<Card>();

            var result = simulator.Simulate(playerA, playerB, communityCards, 1000);

            // With identical hands, should have mostly ties
            Assert.True(result.Ties > result.PlayerAWins + result.PlayerBWins);
        }
    }
}
