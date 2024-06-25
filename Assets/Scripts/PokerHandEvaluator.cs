using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PokerHandEvaluator
{
    public enum HandRank
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush,
        FiveOfAKind  // Added for games with wild cards
    }
    public struct HandEvaluationResult
    {
        public HandRank handRank;
        public int rankWeight;
        public List<Card> bestCards;
    }

    public HandEvaluationResult EvaluateHandWeight(List<Card> hand)
    {
        HandRank handRank = EvaluateHand(hand);
        int rankWeight = CalculateRankWeight(handRank, hand);
        List<Card> bestCards = DetermineBestCards(hand, handRank);
        return new HandEvaluationResult { handRank = handRank, rankWeight = rankWeight, bestCards = bestCards };
    }

    private List<Card> DetermineBestCards(List<Card> hand, HandRank handRank)
    {

        switch (handRank)
        {
            case HandRank.HighCard:
                return DetermineHighCard(hand);
            case HandRank.OnePair:
                return DeterminePairCards(hand);
            case HandRank.TwoPair:
                return DetermineTwoPairCards(hand);
            case HandRank.ThreeOfAKind:
                return DetermineThreeOfAKindCards(hand);
            case HandRank.Straight:
                return DetermineStraightCards(hand);
            case HandRank.Flush:
                return DetermineFlushCards(hand);
            case HandRank.FullHouse:
                return DetermineFullHouseCards(hand);
            case HandRank.FourOfAKind:
                return DetermineFourOfAkindCards(hand);
            case HandRank.StraightFlush:
                return DetermineStraightFlushCards(hand);
            case HandRank.RoyalFlush:
                return DetermineRoyalFlushCards(hand);
            case HandRank.FiveOfAKind:
                return DetermineFiveOfAKindCards(hand);
            default:
                return hand;
        }
    }

    private List<Card> DetermineFiveOfAKindCards(List<Card> hand)
    {
        var groupedByValue = hand.GroupBy(card => card.Value);
        var fiveOfAKindGroup = groupedByValue.FirstOrDefault(group => group.Count() == 5);
        return fiveOfAKindGroup?.ToList() ?? new List<Card>();
    }

    private List<Card> DetermineRoyalFlushCards(List<Card> hand)
    {
        var sortedHand = hand.OrderBy(card => card.HighValue).ToList();
        if (IsRoyalFlush(sortedHand))
            return sortedHand;

        return new List<Card>();
    }
    private bool IsRoyalFlush(List<Card> sortedHand)
    {
        return IsStraightFlush(sortedHand) && sortedHand.Min(card => card.HighValue) == 10;
    }

    private List<Card> DetermineStraightFlushCards(List<Card> hand)
    {
        var sortedHand = hand.OrderBy(card => card.HighValue).ToList();
        if (IsStraightFlush(sortedHand))
            return sortedHand;

        sortedHand = hand.OrderBy(card => card.Value).ToList(); // Ace as 1
        if (IsStraightFlush(sortedHand))
            return sortedHand;

        return new List<Card>();
    }
    private bool IsStraightFlush(List<Card> sortedHand)
    {
        return sortedHand.All(card => card.Suit == sortedHand[0].Suit) && IsStraight(sortedHand);
    }

    private List<Card> DetermineFourOfAkindCards(List<Card> hand)
    {
        var groupedByValue = hand.GroupBy(card => card.Value);
        var fourOfAKindGroup = groupedByValue.FirstOrDefault(group => group.Count() == 4);
        return fourOfAKindGroup?.ToList() ?? new List<Card>();
    }

    private List<Card> DetermineFullHouseCards(List<Card> hand)
    {
        var groupedByValue = hand.GroupBy(card => card.Value);
        var threeOfAKindGroup = groupedByValue.FirstOrDefault(group => group.Count() == 3);
        var pairGroup = groupedByValue.FirstOrDefault(group => group.Count() == 2);
        var fullHouse = new List<Card>();
        if (threeOfAKindGroup != null)
        {
            fullHouse.AddRange(threeOfAKindGroup);
        }
        if (pairGroup != null)
        {
            fullHouse.AddRange(pairGroup);
        }
        return fullHouse;
    }

    private List<Card> DetermineFlushCards(List<Card> hand)
    {
        return hand.OrderByDescending(card => card.HighValue).Take(5).ToList();
    }

    private List<Card> DetermineStraightCards(List<Card> hand)
    {
        var sortedHand = hand.OrderBy(card => card.HighValue).ToList();
        if (IsStraight(sortedHand))
            return sortedHand;

        sortedHand = hand.OrderBy(card => card.Value).ToList(); // Ace as 1
        if (IsStraight(sortedHand))
            return sortedHand;

        return new List<Card>();
    }
    private bool IsStraight(List<Card> sortedHand)
    {
        for (int i = 1; i < sortedHand.Count; i++)
        {
            if (sortedHand[i].HighValue != sortedHand[i - 1].HighValue + 1)
            {
                return false;
            }
        }
        return true;
    }

    private List<Card> DetermineThreeOfAKindCards(List<Card> hand)
    {
        var groupedByValue = hand.GroupBy(card => card.Value);
        var threeOfAKindGroup = groupedByValue.FirstOrDefault(group => group.Count() == 3);
        return threeOfAKindGroup?.ToList() ?? new List<Card>();
    }

    private List<Card> DetermineTwoPairCards(List<Card> hand)
    {
        var groupedByValue = hand.GroupBy(card => card.Value);
        var pairs = groupedByValue.Where(group => group.Count() == 2).Take(2);
        return pairs.SelectMany(pair => pair).ToList();
    }

    private List<Card> DeterminePairCards(List<Card> hand)
    {
        var groupedByValue = hand.GroupBy(card => card.Value);
        var pairGroup = groupedByValue.FirstOrDefault(group => group.Count() == 2);
        return pairGroup?.ToList() ?? new List<Card>();
    }

    private List<Card> DetermineHighCard(List<Card> hand)
    {
        return hand.OrderByDescending(card => card.Value).Take(5).ToList();
    }

    private int CalculateRankWeight(HandRank handRank, List<Card> hand)
    {
        // Assign weights or ranks based on hand rank
        switch (handRank)
        {
            case HandRank.HighCard:
                return 1;
            case HandRank.OnePair:
                return 2;
            case HandRank.TwoPair:
                return 3;
            case HandRank.ThreeOfAKind:
                return 4;
            case HandRank.Straight:
                return 5;
            case HandRank.Flush:
                return 6;
            case HandRank.FullHouse:
                return 7;
            case HandRank.FourOfAKind:
                return 8;
            case HandRank.StraightFlush:
                return 9;
            case HandRank.RoyalFlush:
                return 10;
            case HandRank.FiveOfAKind:
                return 11;
            default:
                return 0; // Default should not be reached if all cases are covered
        }
    }

    public HandRank EvaluateHand(List<Card> hand)
    {
        if (HasFiveOfAKind(hand))
            return HandRank.FiveOfAKind;

        if (HasRoyalFlush(hand))
            return HandRank.RoyalFlush;

        if (HasStraightFlush(hand))
            return HandRank.StraightFlush;

        if (HasFourOfAKind(hand))
            return HandRank.FourOfAKind;

        if (HasFullHouse(hand))
            return HandRank.FullHouse;

        if (HasFlush(hand))
            return HandRank.Flush;

        if (HasStraight(hand))
            return HandRank.Straight;

        if (HasThreeOfAKind(hand))
            return HandRank.ThreeOfAKind;

        if (HasTwoPair(hand))
            return HandRank.TwoPair;

        if (HasOnePair(hand))
            return HandRank.OnePair;

        return HandRank.HighCard;  // Default to High Card if none of the above
    }

    private bool HasFiveOfAKind(List<Card> hand)
    {
        var groupedByValue = hand.GroupBy(card => card.Value);
        return groupedByValue.Any(group => group.Count() == 5);
    }

    private bool HasRoyalFlush(List<Card> hand)
    {
        // Check for flush
        if (!HasFlush(hand))
        {
            return false;
        }
        // Sort hand by rank
        hand.Sort((a, b) => a.Value.CompareTo(b.Value));

        // Check for Royal Flush (A, K, Q, J, 10 in the same suit)
        return hand[0].Value == 01 &&    // Ace
               hand[1].Value == 10 &&   // 10
               hand[2].Value == 11 &&   // Jack (J)
               hand[3].Value == 12 &&   // Queen (Q)
               hand[4].Value == 13;     //King (K)

    }

    private bool HasStraightFlush(List<Card> hand)
    {
        // Check for flush
        if (!HasFlush(hand))
        {
            return false;
        }

        // Check for straight
        if (!HasStraight(hand))
        {
            return false;
        }

        return true;
    }

    private bool HasFourOfAKind(List<Card> hand)
    {
        return hand.GroupBy(card => card.Value).Any(group => group.Count() == 4);
    }
    private bool HasFullHouse(List<Card> hand)
    {
        var groupedByRank = hand.GroupBy(card => card.Value);
        return groupedByRank.Any(group => group.Count() == 3) && groupedByRank.Any(group => group.Count() == 2);
    }
    private bool HasFlush(List<Card> hand)
    {
        return hand.All(card => card.Suit == hand.First().Suit);
    }
    private bool HasStraight(List<Card> hand)
    {
        // Sort hand by rank
        hand.Sort((a, b) => a.Value.CompareTo(b.Value));

        // Check for straight
        for (int i = 0; i < hand.Count - 1; i++)
        {
            if (hand[i + 1].Value != hand[i].Value + 1)
            {
                return false;
            }
        }

        return true;
    }
    private bool HasThreeOfAKind(List<Card> hand)
    {
        return hand.GroupBy(card => card.Value).Any(group => group.Count() == 3);
    }
    private bool HasTwoPair(List<Card> hand)
    {
        return hand.GroupBy(card => card.Value).Count(group => group.Count() == 2) == 2;
    }
    private bool HasOnePair(List<Card> hand)
    {
        return hand.GroupBy(card => card.Value).Any(group => group.Count() == 2);
    }

    public int CompareHands(List<Card> hand1, List<Card> hand2)
    {
        var result1 = EvaluateHandWeight(hand1);
        var result2 = EvaluateHandWeight(hand2);

        if (result1.rankWeight > result2.rankWeight)
        {
            return 1; // Hand1 wins
        }
        else if (result1.rankWeight < result2.rankWeight)
        {
            return -1; // Hand2 wins
        }
        else
        {
            // If both hands have the same rank weight, compare the best cards
            for (int i = 0; i < result1.bestCards.Count; i++)
            {
                if (result1.bestCards[i].Value > result2.bestCards[i].Value)
                    return 1; // Hand1 wins
                else if (result1.bestCards[i].Value < result2.bestCards[i].Value)
                    return -1; // Hand2 wins
            }
            return 0; // It's a tie
        }
    }
}

