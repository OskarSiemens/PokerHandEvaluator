using PokerHandEvaluator.Models;
using PokerHandEvaluator.Evaluators;
using System.Collections.Generic;
using System;

namespace PokerHandEvaluator.Simulators
{
    /// <summary>
    /// Simulates poker hands using Effective Hand Strength (EHS) algorithm
    /// EHS combines current hand strength with future improvement potential
    /// </summary>
    public class EHSHandSimulator
    {
        private readonly GameType _gameType;
        private readonly EffectiveHandStrengthCalculator _ehsCalculator;

        public EHSHandSimulator(GameType gameType)
        {
            _gameType = gameType;
            _ehsCalculator = new EffectiveHandStrengthCalculator(gameType);
        }

        /// <summary>
        /// Simulates hand outcomes using EHS algorithm
        /// </summary>
        /// <param name="playerAHand">Player A's hole cards</param>
        /// <param name="playerBHand">Player B's hole cards</param>
        /// <param name="communityCards">Known community cards (0-4 cards)</param>
        /// <param name="numberOfSimulations">Number of simulations to run</param>
        /// <param name="ehsSamples">Number of opponent hands to sample per EHS calculation</param>
        /// <returns>EHS simulation result</returns>
        public EHSSimulationResult Simulate(Hand playerAHand, Hand playerBHand, List<Card> communityCards, 
            int numberOfSimulations, int ehsSamples = 500)
        {
            if (numberOfSimulations <= 0)
                throw new ArgumentException("Number of simulations must be greater than 0");

            if (ehsSamples <= 0)
                throw new ArgumentException("EHS samples must be greater than 0");

            double playerAEHS = _ehsCalculator.CalculateEHS(playerAHand, communityCards, ehsSamples);
            double playerBEHS = _ehsCalculator.CalculateEHS(playerBHand, communityCards, ehsSamples);

            double playerAHandStrength = _ehsCalculator.CalculateHandStrength(playerAHand, communityCards, ehsSamples);
            double playerBHandStrength = _ehsCalculator.CalculateHandStrength(playerBHand, communityCards, ehsSamples);

            double playerAPosPot = communityCards.Count < 5 
                ? _ehsCalculator.CalculatePositivePotential(playerAHand, communityCards, ehsSamples) 
                : 0;
            double playerBPosPot = communityCards.Count < 5 
                ? _ehsCalculator.CalculatePositivePotential(playerBHand, communityCards, ehsSamples) 
                : 0;

            double playerANegPot = communityCards.Count < 5 
                ? _ehsCalculator.CalculateNegativePotential(playerAHand, communityCards, ehsSamples) 
                : 0;
            double playerBNegPot = communityCards.Count < 5 
                ? _ehsCalculator.CalculateNegativePotential(playerBHand, communityCards, ehsSamples) 
                : 0;

            // Determine winner based on EHS
            int playerAWins = 0;
            int playerBWins = 0;
            int ties = 0;

            for (int i = 0; i < numberOfSimulations; i++)
            {
                if (playerAEHS > playerBEHS)
                    playerAWins++;
                else if (playerBEHS > playerAEHS)
                    playerBWins++;
                else
                    ties++;
            }

            return new EHSSimulationResult
            {
                PlayerAWins = playerAWins,
                PlayerBWins = playerBWins,
                Ties = ties,
                TotalSimulations = numberOfSimulations,
                PlayerAEHS = playerAEHS,
                PlayerBEHS = playerBEHS,
                PlayerAHandStrength = playerAHandStrength,
                PlayerBHandStrength = playerBHandStrength,
                PlayerAPositivePotential = playerAPosPot,
                PlayerBPositivePotential = playerBPosPot,
                PlayerANegativePotential = playerANegPot,
                PlayerBNegativePotential = playerBNegPot
            };
        }
    }

    public class EHSSimulationResult
    {
        public int PlayerAWins { get; set; }
        public int PlayerBWins { get; set; }
        public int Ties { get; set; }
        public int TotalSimulations { get; set; }

        // EHS Values (0-1)
        public double PlayerAEHS { get; set; }
        public double PlayerBEHS { get; set; }

        // Hand Strength (0-1)
        public double PlayerAHandStrength { get; set; }
        public double PlayerBHandStrength { get; set; }

        // Positive Potential (0-1)
        public double PlayerAPositivePotential { get; set; }
        public double PlayerBPositivePotential { get; set; }

        // Negative Potential (0-1)
        public double PlayerANegativePotential { get; set; }
        public double PlayerBNegativePotential { get; set; }

        public override string ToString()
        {
            return $"Player A Wins: {PlayerAWins}, Player B Wins: {PlayerBWins}, Ties: {Ties}\n" +
                   $"Player A EHS: {PlayerAEHS:P2}, Player B EHS: {PlayerBEHS:P2}\n" +
                   $"Player A HS: {PlayerAHandStrength:P2}, Player B HS: {PlayerBHandStrength:P2}\n" +
                   $"Player A PPot: {PlayerAPositivePotential:P2}, Player B PPot: {PlayerBPositivePotential:P2}\n" +
                   $"Player A NPot: {PlayerANegativePotential:P2}, Player B NPot: {PlayerBNegativePotential:P2}";
        }
    }
}
