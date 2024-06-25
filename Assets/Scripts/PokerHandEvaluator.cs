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
        // Check if the hand has A, K, Q, J, 10 of the same suit
        return false;
    }

    private bool HasStraightFlush(List<Card> hand)
    {
        // Check if the hand has five consecutive cards of the same suit
        return false;
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

        return false;
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

