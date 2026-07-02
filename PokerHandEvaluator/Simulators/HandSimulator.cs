using PokerHandEvaluator.Models;
using PokerHandEvaluator.Evaluators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerHandEvaluator.Simulators
{
    /// <summary>
    /// Simulates poker hands to determine win/tie outcomes
    /// </summary>
    public class HandSimulator
    {
        private readonly GameType _gameType;
        private readonly IHandEvaluator _evaluator;
        private readonly Random _random = new Random();

        public HandSimulator(GameType gameType)
        {
            _gameType = gameType;
            _evaluator = gameType switch
            {
                GameType.TexasHoldEm => new TexasHoldEmEvaluator(),
                GameType.PLO4 => new PLO4Evaluator(),
                _ => throw new ArgumentException($"Unknown game type: {gameType}")
            };
        }

        /// <summary>
        /// Simulates hand outcomes
        /// </summary>
        /// <param name="playerAHand">Player A's hole cards</param>
        /// <param name="playerBHand">Player B's hole cards</param>
        /// <param name="communityCards">Known community cards (0-4 cards)</param>
        /// <param name="numberOfSimulations">Number of simulations to run</param>
        /// <returns>Simulation result with win/tie counts</returns>
        public SimulationResult Simulate(Hand playerAHand, Hand playerBHand, List<Card> communityCards, int numberOfSimulations)
        {
            if (numberOfSimulations <= 0)
                throw new ArgumentException("Number of simulations must be greater than 0");
            if (playerAHand.Compare(playerBHand) > 0)
                throw new ArgumentException("At least one card is the same between player A and B");

            int playerAWins = 0;
            int playerBWins = 0;
            int ties = 0;

            var deck = CreateDeck();
            RemoveUsedCards(deck, playerAHand, playerBHand, communityCards);

            for (int i = 0; i < numberOfSimulations; i++)
            {
                var deckBeforeDealing = new List<Card>(deck);
                var (finalCommunity, deckCopy) = DealRemainingCards(deck, communityCards);

                var playerARanking = _evaluator.Evaluate(playerAHand, finalCommunity);
                var playerBRanking = _evaluator.Evaluate(playerBHand, finalCommunity);

                var comparison = playerARanking.CompareTo(playerBRanking);

                if (comparison > 0)
                    playerAWins++;
                else if (comparison < 0)
                    playerBWins++;
                else
                    ties++;

                deck = deckBeforeDealing;
            }

            return new SimulationResult
            {
                PlayerAWins = playerAWins,
                PlayerBWins = playerBWins,
                Ties = ties,
                TotalSimulations = numberOfSimulations
            };
        }

        private List<Card> CreateDeck()
        {
            var deck = new List<Card>();
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    deck.Add(new Card(rank, suit));
                }
            }
            return deck;
        }

        private void RemoveUsedCards(List<Card> deck, Hand playerAHand, Hand playerBHand, List<Card> communityCards)
        {
            var usedCards = playerAHand.Cards.Concat(playerBHand.Cards).Concat(communityCards).ToList();
            foreach (var card in usedCards)
            {
                deck.Remove(card);
            }
        }

        private (List<Card> finalCommunity, List<Card> newDeck) DealRemainingCards(List<Card> deck, List<Card> knownCommunity)
        {
            var deckCopy = new List<Card>(deck);
            var remaining = 5 - knownCommunity.Count;

            var dealtCards = new List<Card>();
            for (int i = 0; i < remaining; i++)
            {
                var index = _random.Next(deckCopy.Count);
                dealtCards.Add(deckCopy[index]);
                deckCopy.RemoveAt(index);
            }

            var finalCommunity = new List<Card>(knownCommunity);
            finalCommunity.AddRange(dealtCards);

            return (finalCommunity, deckCopy);
        }
    }

    public class SimulationResult
    {
        public int PlayerAWins { get; set; }
        public int PlayerBWins { get; set; }
        public int Ties { get; set; }
        public int TotalSimulations { get; set; }

        public override string ToString() =>
            $"Player A Wins: {PlayerAWins}, Player B Wins: {PlayerBWins}, Ties: {Ties}";
    }
}
