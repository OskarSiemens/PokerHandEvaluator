using PokerHandEvaluator.Models;
using System.Collections.Generic;

namespace PokerHandEvaluator.Evaluators
{
    public interface IHandEvaluator
    {
        HandRanking Evaluate(Hand playerHand, List<Card> communityCards);
    }
}
