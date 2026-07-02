using PokerHandEvaluator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerHandEvaluator.Evaluators
{
    /// <summary>
    /// Evaluates PLO4 hands (4 hole cards, must use exactly 2 with 3 from community)
    /// </summary>
    public class PLO4Evaluator : IHandEvaluator
    {
        private readonly TexasHoldEmEvaluator _baseEvaluator = new TexasHoldEmEvaluator();

        public HandRanking Evaluate(Hand playerHand, List<Card> communityCards)
        {
            if (playerHand.Cards.Count != 4)
                throw new ArgumentException("PLO4 requires exactly 4 hole cards");

            if (communityCards.Count < 3 || communityCards.Count > 5)
                throw new ArgumentException("Community cards must be between 3 and 5");

            // Get all combinations of 2 cards from hand (must use exactly 2)
            var holeCardCombinations = GetCombinations(playerHand.Cards, 2);

            // Get all combinations of 3 cards from community (must use exactly 3)
            var communityCombinations = GetCombinations(communityCards, 3);

            var bestRanking = (HandRanking?)null;

            foreach (var holeCombo in holeCardCombinations)
            {
                foreach (var communityCombo in communityCombinations)
                {
                    var hand = new Hand(holeCombo.Concat(communityCombo).ToArray());
                    var ranking = _baseEvaluator.Evaluate(hand, new List<Card>());

                    if (bestRanking == null || ranking.CompareTo(bestRanking) > 0)
                    {
                        bestRanking = ranking;
                    }
                }
            }

            return bestRanking ?? throw new InvalidOperationException("Unable to evaluate hand");
        }

        private List<List<Card>> GetCombinations(List<Card> cards, int size)
        {
            var result = new List<List<Card>>();
            var combination = new Card[size];
            GenerateCombinations(cards, combination, 0, 0, result);
            return result;
        }

        private void GenerateCombinations(List<Card> cards, Card[] combination, int start, int index, List<List<Card>> result)
        {
            if (index == combination.Length)
            {
                result.Add(combination.ToList());
                return;
            }

            for (int i = start; i < cards.Count; i++)
            {
                combination[index] = cards[i];
                GenerateCombinations(cards, combination, i + 1, index + 1, result);
            }
        }
    }
}
