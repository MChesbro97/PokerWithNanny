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

        return new List<Card>;
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
        // Implement logic to check for five of a kind, including wild cards if any
        // Example: Four cards of the same rank plus a wild card
        // This will depend on your specific game rules for wild cards
        // Return true or false accordingly
        return false;
    }

    // Implement methods for checking other hand types (Royal Flush, Straight Flush, etc.)
    // These methods should check if the hand meets the criteria for each hand type

    // Example methods:
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
        return hand[0].Value == "01" &&    // Ace
               hand[1].Value == "10" &&   // 10
               hand[2].Value == "11" &&   // Jack (J)
               hand[3].Value == "12" &&   // Queen (Q)
               hand[4].Value == "13";     //King (K)

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
        var groupedByRank = hand.GroupBy(card => card.Value);
        return groupedByRank.Any(group => group.Count() == 4);
    }
    private bool HasFullHouse(List<Card> hand)
    {
        var groupedByRank = hand.GroupBy(card => card.Value);
        return groupedByRank.Any(group => group.Count() == 3) &&
               groupedByRank.Any(group => group.Count() == 2);
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
        var groupedByRank = hand.GroupBy(card => card.Value);
        return groupedByRank.Any(group => group.Count() == 3);
    }
    private bool HasTwoPair(List<Card> hand)
    {
        var groupedByRank = hand.GroupBy(card => card.Value);
        int pairCount = groupedByRank.Count(group => group.Count() == 2);
        return pairCount == 2;
    }
    private bool HasOnePair(List<Card> hand)
    {
        var groupedByValue = hand.GroupBy(card => card.Value);
        return groupedByValue.Any(group => group.Count() == 2);
    }
}

