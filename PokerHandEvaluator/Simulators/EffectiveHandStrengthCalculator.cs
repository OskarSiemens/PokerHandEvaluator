using PokerHandEvaluator.Models;
using PokerHandEvaluator.Evaluators;
using System.Collections.Generic;
using System;
using System.Linq;

namespace PokerHandEvaluator.Simulators
{
    /// <summary>
    /// Calculates Effective Hand Strength (EHS) for poker hands
    /// EHS combines hand strength (current probability of winning) with hand potential (future improvement probability)
    /// </summary>
    public class EffectiveHandStrengthCalculator
    {
        private readonly IHandEvaluator _evaluator;
        private readonly GameType _gameType;

        public EffectiveHandStrengthCalculator(GameType gameType)
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
        /// Calculates EHS for a given hand against opponent range
        /// </summary>
        /// <param name="playerHand">Player's hole cards</param>
        /// <param name="communityCards">Known community cards (0-4)</param>
        /// <param name="opponentHandSamples">Number of random opponent hands to sample</param>
        /// <returns>EHS value between 0 and 1</returns>
        public double CalculateEHS(Hand playerHand, List<Card> communityCards, int opponentHandSamples = 1000)
        {
            if (opponentHandSamples <= 0)
                throw new ArgumentException("Opponent hand samples must be greater than 0");

            var deck = CreateDeck();
            RemoveUsedCards(deck, playerHand, communityCards);

            int handStrengthWins = 0;
            int handStrengthLosses = 0;
            int handStrengthTies = 0;
            double positivePotential = 0;
            double negativePotential = 0;

            var random = new Random();

            // Sample random opponent hands
            for (int i = 0; i < opponentHandSamples; i++)
            {
                var opponentHand = GenerateRandomHand(deck, playerHand, communityCards);
                var deckCopy = new List<Card>(deck);

                // Calculate current hand strength
                var playerRanking = _evaluator.Evaluate(playerHand, communityCards);
                var opponentRanking = _evaluator.Evaluate(opponentHand, communityCards);

                int comparison = playerRanking.CompareTo(opponentRanking);

                if (comparison > 0)
                    handStrengthWins++;
                else if (comparison < 0)
                    handStrengthLosses++;
                else
                    handStrengthTies++;

                // Calculate hand potential (improvement to river)
                if (communityCards.Count < 5)
                {
                    var riverCommunity = CompleteToRiver(deckCopy, communityCards);
                    var playerFinalRanking = _evaluator.Evaluate(playerHand, riverCommunity);
                    var opponentFinalRanking = _evaluator.Evaluate(opponentHand, riverCommunity);

                    int finalComparison = playerFinalRanking.CompareTo(opponentFinalRanking);

                    // Positive potential: inferior hand becomes superior
                    if (comparison < 0 && finalComparison > 0)
                        positivePotential++;

                    // Negative potential: superior hand becomes inferior
                    if (comparison > 0 && finalComparison < 0)
                        negativePotential++;
                }
            }

            double handStrength = (double)handStrengthWins / (handStrengthWins + handStrengthLosses + handStrengthTies);
            double pPot = positivePotential / opponentHandSamples;
            double nPot = negativePotential / opponentHandSamples;

            // EHS = HS × (1 - NPot) + (1 - HS) × PPot
            double ehs = (handStrength * (1 - nPot)) + ((1 - handStrength) * pPot);

            return Math.Clamp(ehs, 0, 1);
        }

        /// <summary>
        /// Calculates hand strength (probability of current hand being best)
        /// </summary>
        public double CalculateHandStrength(Hand playerHand, List<Card> communityCards, int opponentHandSamples = 1000)
        {
            if (opponentHandSamples <= 0)
                throw new ArgumentException("Opponent hand samples must be greater than 0");

            var deck = CreateDeck();
            RemoveUsedCards(deck, playerHand, communityCards);

            int wins = 0;
            int total = 0;

            for (int i = 0; i < opponentHandSamples; i++)
            {
                var opponentHand = GenerateRandomHand(deck, playerHand, communityCards);

                var playerRanking = _evaluator.Evaluate(playerHand, communityCards);
                var opponentRanking = _evaluator.Evaluate(opponentHand, communityCards);

                if (playerRanking.CompareTo(opponentRanking) > 0)
                    wins++;

                total++;
            }

            return (double)wins / total;
        }

        /// <summary>
        /// Calculates positive potential (chance inferior hand improves to be best)
        /// </summary>
        public double CalculatePositivePotential(Hand playerHand, List<Card> communityCards, int samples = 1000)
        {
            if (communityCards.Count >= 5)
                return 0; // No potential on river

            var deck = CreateDeck();
            RemoveUsedCards(deck, playerHand, communityCards);

            int improvements = 0;

            for (int i = 0; i < samples; i++)
            {
                var opponentHand = GenerateRandomHand(deck, playerHand, communityCards);
                var deckCopy = new List<Card>(deck);

                var playerCurrent = _evaluator.Evaluate(playerHand, communityCards);
                var opponentCurrent = _evaluator.Evaluate(opponentHand, communityCards);

                // Only count if currently behind
                if (playerCurrent.CompareTo(opponentCurrent) < 0)
                {
                    var riverCommunity = CompleteToRiver(deckCopy, communityCards);
                    var playerFinal = _evaluator.Evaluate(playerHand, riverCommunity);
                    var opponentFinal = _evaluator.Evaluate(opponentHand, riverCommunity);

                    // Check if now ahead
                    if (playerFinal.CompareTo(opponentFinal) > 0)
                        improvements++;
                }
            }

            return (double)improvements / samples;
        }

        /// <summary>
        /// Calculates negative potential (chance superior hand becomes inferior)
        /// </summary>
        public double CalculateNegativePotential(Hand playerHand, List<Card> communityCards, int samples = 1000)
        {
            if (communityCards.Count >= 5)
                return 0; // No potential on river

            var deck = CreateDeck();
            RemoveUsedCards(deck, playerHand, communityCards);

            int deteriorations = 0;

            for (int i = 0; i < samples; i++)
            {
                var opponentHand = GenerateRandomHand(deck, playerHand, communityCards);
                var deckCopy = new List<Card>(deck);

                var playerCurrent = _evaluator.Evaluate(playerHand, communityCards);
                var opponentCurrent = _evaluator.Evaluate(opponentHand, communityCards);

                // Only count if currently ahead
                if (playerCurrent.CompareTo(opponentCurrent) > 0)
                {
                    var riverCommunity = CompleteToRiver(deckCopy, communityCards);
                    var playerFinal = _evaluator.Evaluate(playerHand, riverCommunity);
                    var opponentFinal = _evaluator.Evaluate(opponentHand, riverCommunity);

                    // Check if now behind
                    if (playerFinal.CompareTo(opponentFinal) < 0)
                        deteriorations++;
                }
            }

            return (double)deteriorations / samples;
        }

        private Hand GenerateRandomHand(List<Card> deck, Hand playerHand, List<Card> communityCards)
        {
            var availableCards = deck.Where(c => 
                !playerHand.Cards.Contains(c) && 
                !communityCards.Contains(c)
            ).ToList();

            var random = new Random();
            int holeCards = _gameType == GameType.TexasHoldEm ? 2 : 4;
            var opponentCards = new List<Card>();

            for (int i = 0; i < holeCards; i++)
            {
                int index = random.Next(availableCards.Count);
                opponentCards.Add(availableCards[index]);
                availableCards.RemoveAt(index);
            }

            return new Hand(opponentCards.ToArray());
        }

        private List<Card> CompleteToRiver(List<Card> deck, List<Card> knownCommunity)
        {
            var deckCopy = new List<Card>(deck);
            var community = new List<Card>(knownCommunity);
            var random = new Random();
            var remaining = 5 - community.Count;

            for (int i = 0; i < remaining; i++)
            {
                int index = random.Next(deckCopy.Count);
                community.Add(deckCopy[index]);
                deckCopy.RemoveAt(index);
            }

            return community;
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

        private void RemoveUsedCards(List<Card> deck, Hand playerHand, List<Card> communityCards)
        {
            var usedCards = playerHand.Cards.Concat(communityCards).ToList();
            foreach (var card in usedCards)
            {
                deck.Remove(card);
            }
        }
    }
}
