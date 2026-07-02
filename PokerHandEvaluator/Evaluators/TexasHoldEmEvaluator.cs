using PokerHandEvaluator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerHandEvaluator.Evaluators
{
    /// <summary>
    /// Evaluates Texas Hold'em hands (2 hole cards, best 5 from 7 total)
    /// </summary>
    public class TexasHoldEmEvaluator : IHandEvaluator
    {
        public HandRanking Evaluate(Hand playerHand, List<Card> communityCards)
        {
            if (playerHand.Cards.Count != 2)
                throw new ArgumentException("Texas Hold'em requires exactly 2 hole cards");

            var allCards = playerHand.Cards.Concat(communityCards).ToList();
            var bestHand = FindBestFiveCardHand(allCards);
            return EvaluateHand(bestHand);
        }

        private List<Card> FindBestFiveCardHand(List<Card> cards)
        {
            var combinations = GetCombinations(cards, 5);
            var evaluations = combinations.Select(c => (hand: c, ranking: EvaluateHand(c)));
            var retVal = evaluations.OrderBy(e => e.ranking, new HandRankingComparer());
                //.First().hand;
                var a = retVal.First().hand;
            return a;
        }

        private HandRanking EvaluateHand(List<Card> cards)
        {
            var sorted = cards.OrderByDescending(c => c.Rank).ToList();

            if (IsRoyalFlush(sorted, out var royalFlushRanks))
                return new HandRanking(HandRankType.RoyalFlush, royalFlushRanks);

            if (IsStraightFlush(sorted, out var straightFlushRanks))
                return new HandRanking(HandRankType.StraightFlush, straightFlushRanks);

            if (IsFourOfAKind(sorted, out var fourRanks))
                return new HandRanking(HandRankType.FourOfAKind, fourRanks);

            if (IsFullHouse(sorted, out var fullHouseRanks))
                return new HandRanking(HandRankType.FullHouse, fullHouseRanks);

            if (IsFlush(sorted, out var flushRanks))
                return new HandRanking(HandRankType.Flush, flushRanks);

            if (IsStraight(sorted, out var straightRanks))
                return new HandRanking(HandRankType.Straight, straightRanks);

            if (IsThreeOfAKind(sorted, out var threeRanks))
                return new HandRanking(HandRankType.ThreeOfAKind, threeRanks);

            if (IsTwoPair(sorted, out var twoPairRanks))
                return new HandRanking(HandRankType.TwoPair, twoPairRanks);

            if (IsOnePair(sorted, out var pairRanks))
                return new HandRanking(HandRankType.OnePair, pairRanks);

            return new HandRanking(HandRankType.HighCard, sorted.Select(c => c.Rank).ToList());
        }

        private bool IsRoyalFlush(List<Card> cards, out List<Rank> ranks)
        {
            ranks = new List<Rank>();
            if (!IsFlush(cards, out _))
                return false;

            if (!IsStraight(cards, out var straightRanks))
                return false;

            return straightRanks[0] == Rank.Ace;
        }

        private bool IsStraightFlush(List<Card> cards, out List<Rank> ranks)
        {
            ranks = new List<Rank>();
            if (!IsFlush(cards, out _))
                return false;

            return IsStraight(cards, out ranks);
        }

        private bool IsFourOfAKind(List<Card> cards, out List<Rank> ranks)
        {
            ranks = new List<Rank>();
            var grouped = cards.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ToList();

            if (grouped[0].Count() == 4)
            {
                ranks.Add(grouped[0].Key);
                ranks.AddRange(grouped.Skip(1).Select(g => g.Key).OrderByDescending(r => r));
                return true;
            }

            return false;
        }

        private bool IsFullHouse(List<Card> cards, out List<Rank> ranks)
        {
            ranks = new List<Rank>();
            var grouped = cards.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ToList();

            if (grouped[0].Count() == 3 && grouped[1].Count() >= 2)
            {
                ranks.Add(grouped[0].Key);
                ranks.Add(grouped.Where(g => g.Count() >= 2).Skip(1).First().Key);
                return true;
            }

            return false;
        }

        private bool IsFlush(List<Card> cards, out List<Rank> ranks)
        {
            ranks = new List<Rank>();
            var suitGroups = cards.GroupBy(c => c.Suit).ToList();

            if (suitGroups.Any(g => g.Count() >= 5))
            {
                var flush = suitGroups.First(g => g.Count() >= 5)
                    .OrderByDescending(c => c.Rank)
                    .Take(5)
                    .ToList();
                ranks = flush.Select(c => c.Rank).ToList();
                return true;
            }

            return false;
        }

        private bool IsStraight(List<Card> cards, out List<Rank> ranks)
        {
            ranks = new List<Rank>();
            var uniqueRanks = cards.Select(c => c.Rank).Distinct().OrderByDescending(r => r).ToList();

            // Check for regular straight
            for (int i = 0; i <= uniqueRanks.Count - 5; i++)
            {
                if ((int)uniqueRanks[i] - (int)uniqueRanks[i + 4] == 4)
                {
                    ranks = uniqueRanks.Skip(i).Take(5).ToList();
                    return true;
                }
            }

            // Check for wheel (A-2-3-4-5)
            if (uniqueRanks.Contains(Rank.Ace) && uniqueRanks.Contains(Rank.Two) &&
                uniqueRanks.Contains(Rank.Three) && uniqueRanks.Contains(Rank.Four) &&
                uniqueRanks.Contains(Rank.Five))
            {
                ranks = new List<Rank> { Rank.Five, Rank.Four, Rank.Three, Rank.Two, Rank.Ace };
                return true;
            }

            return false;
        }

        private bool IsThreeOfAKind(List<Card> cards, out List<Rank> ranks)
        {
            ranks = new List<Rank>();
            var grouped = cards.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ToList();

            if (grouped[0].Count() == 3)
            {
                ranks.Add(grouped[0].Key);
                ranks.AddRange(grouped.Skip(1).Select(g => g.Key).OrderByDescending(r => r).Take(2));
                return true;
            }

            return false;
        }

        private bool IsTwoPair(List<Card> cards, out List<Rank> ranks)
        {
            ranks = new List<Rank>();
            var grouped = cards.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ToList();
            var pairs = grouped.Where(g => g.Count() >= 2).OrderByDescending(g => g.Key).ToList();

            if (pairs.Count >= 2)
            {
                ranks.Add(pairs[0].Key);
                ranks.Add(pairs[1].Key);
                ranks.AddRange(grouped.Where(g => g.Count() == 1).Select(g => g.Key).OrderByDescending(r => r).Take(1));
                return true;
            }

            return false;
        }

        private bool IsOnePair(List<Card> cards, out List<Rank> ranks)
        {
            ranks = new List<Rank>();
            var grouped = cards.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ToList();

            if (grouped[0].Count() == 2)
            {
                ranks.Add(grouped[0].Key);
                ranks.AddRange(grouped.Skip(1).Select(g => g.Key).OrderByDescending(r => r).Take(3));
                return true;
            }

            return false;
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

    internal class HandRankingComparer : IComparer<HandRanking>
    {
        public int Compare(HandRanking? x, HandRanking? y)
        {
            if (x == null || y == null)
                return 0;
            return x.CompareTo(y);
        }
    }
}
