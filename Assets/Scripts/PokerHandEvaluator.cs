using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PokerHandEvaluator : MonoBehaviour
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
    private HandEvaluationResult EvaluateHandWithWilds(List<Card> hand)
    {
        // Handle wild cards
        List<Card> nonWildCards = hand.Where(card => !card.IsWild).ToList();
        int wildCount = hand.Count(card => card.IsWild);

        if (wildCount == 0)
        {
            return new HandEvaluationResult
            {
                handRank = EvaluateHand(nonWildCards),
                bestCards = nonWildCards
            };
        }

        List<Card> bestHand = null;
        HandRank bestRank = HandRank.HighCard;
        foreach (var wildHand in GenerateWildCardCombinations(nonWildCards, wildCount))
        {
            HandRank rank = EvaluateHand(wildHand);
            if (rank > bestRank || (rank == bestRank && CompareBestCards(wildHand, bestHand) > 0))
            {
                bestRank = rank;
                bestHand = wildHand;
            }
        }

        return new HandEvaluationResult { handRank = bestRank, bestCards = bestHand };
    }

    private IEnumerable<List<Card>> GenerateWildCardCombinations(List<Card> nonWildCards, int wildCount)
    {
        // Generate all possible combinations of hands with wild cards replacing any other card
        // For simplicity, only consider replacements that could form valid hands

        // Example implementation, could be improved
        List<Card> allPossibleCards = GetAllPossibleCards();
        foreach (var combination in Combinations(allPossibleCards, wildCount))
        {
            List<Card> handWithWilds = new List<Card>(nonWildCards);
            handWithWilds.AddRange(combination);
            yield return handWithWilds;
        }
    }
    private List<Card> GetAllPossibleCards()
    {
        // Generate all possible cards in a deck (excluding jokers)
        var suits = new[] { "Hearts", "Diamonds", "Clubs", "Spades" };
        var possibleCards = new List<Card>();
        foreach (var suit in suits)
        {
            for (int value = 1; value <= 13; value++)
            {
                possibleCards.Add(new Card(suit, value, null, null));
            }
        }
        return possibleCards;
    }
    private IEnumerable<IEnumerable<T>> Combinations<T>(IEnumerable<T> elements, int k)
    {
        if (k == 0)
        {
            yield return Enumerable.Empty<T>();
        }
        else
        {
            int i = 0;
            foreach (var element in elements)
            {
                foreach (var combination in Combinations(elements.Skip(i + 1), k - 1))
                {
                    yield return new[] { element }.Concat(combination);
                }
                i++;
            }
        }
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
                return DetermineFourOfAKindCards(hand);
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
        int wildCount = hand.Count(card => card.IsWild);
        var fiveOfAKindGroup = groupedByValue.FirstOrDefault(group => group.Count() + wildCount >= 5); // Changed condition to >= 5

        if (fiveOfAKindGroup != null)
        {
            // Determine how many wild cards are needed to complete the five of a kind
            wildCount = 5 - fiveOfAKindGroup.Count();
            var actualFiveOfAKind = fiveOfAKindGroup.ToList();

            // Add wild cards if necessary
            if (wildCount > 0)
            {
                var wildCards = hand.Where(card => card.IsWild).Take(wildCount);
                actualFiveOfAKind.AddRange(wildCards);
            }

            // Sort by value descending
            return actualFiveOfAKind.OrderByDescending(card => card.Value).ToList();
        }

        return new List<Card>();
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

    private List<Card> DetermineFourOfAKindCards(List<Card> hand)
    {
        var groupedByValue = hand.GroupBy(card => card.Value);
        int wildCount = hand.Count(card => card.IsWild);
        var fourOfAKindGroup = groupedByValue.FirstOrDefault(group => group.Count() + wildCount >= 4); // Changed condition to >= 4

        if (fourOfAKindGroup != null)
        {
            // Find the kicker(s) for the four of a kind
            var kickers = hand.Where(card => card.Value != fourOfAKindGroup.Key).OrderByDescending(card => card.Value).Take(1); // Adjusted to take one kicker

            // If there are wild cards in the four of a kind, adjust the kicker accordingly
            wildCount = hand.Count(card => card.IsWild && card.Value == fourOfAKindGroup.Key);
            if (wildCount > 0)
            {
                // Remove as many wild cards as needed from the kicker list
                kickers = kickers.Skip(wildCount);
            }

            return fourOfAKindGroup.Concat(kickers).ToList();
        }

        return new List<Card>();
    }

    private List<Card> DetermineFullHouseCards(List<Card> hand)
    {
        var groupedByValue = hand.GroupBy(card => card.Value);
        int wildCount = hand.Count(card => card.IsWild);
        var threeOfAKindGroup = groupedByValue.FirstOrDefault(group => group.Count() + wildCount >= 3); // Changed condition to >= 3
        var pairGroup = groupedByValue.FirstOrDefault(group => group.Count() + wildCount >= 2); // Changed condition to >= 2

        if (threeOfAKindGroup != null && pairGroup != null)
        {
            // Adjust for wild cards in three of a kind
            int wildThreeCount = hand.Count(card => card.IsWild && card.Value == threeOfAKindGroup.Key);
            int wildPairCount = hand.Count(card => card.IsWild && card.Value == pairGroup.Key);

            // Determine the actual cards for three of a kind and pair
            var actualThreeOfAKind = threeOfAKindGroup.Take(3 - wildThreeCount);
            var actualPair = pairGroup.Take(2 - wildPairCount);

            // Concatenate and order by value
            return actualThreeOfAKind.Concat(actualPair).OrderByDescending(card => card.Value).ToList();
        }

        return new List<Card>();
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
        Debug.Log($"Determining Three of a Kind cards. Hand: [{string.Join(", ", hand.Select(card => card.ToString()))}]");

        var groupedByValue = hand.GroupBy(card => card.Value);
        int wildCount = hand.Count(card => card.IsWild);
        var threeOfAKindGroup = groupedByValue.FirstOrDefault(group => group.Count() + wildCount >= 3); // Adjusted to >= 3

        if (threeOfAKindGroup != null)
        {
            Debug.Log($"Found Three of a Kind: [{string.Join(", ", threeOfAKindGroup.Select(card => card.ToString()))}]");

            // Determine how many wild cards are needed to complete the three of a kind
            wildCount = 3 - threeOfAKindGroup.Count();
            var actualThreeOfAKind = threeOfAKindGroup.ToList();

            // Add wild cards if necessary
            if (wildCount > 0)
            {
                var wildCards = hand.Where(card => card.IsWild).Take(wildCount);
                actualThreeOfAKind.AddRange(wildCards);
                Debug.Log($"Added {wildCount} wild card(s): [{string.Join(", ", wildCards.Select(card => card.ToString()))}]");
            }

            // Sort by value descending
            var sortedCards = actualThreeOfAKind.OrderByDescending(card => card.Value).ToList();
            Debug.Log($"Sorted Three of a Kind cards: [{string.Join(", ", sortedCards.Select(card => card.ToString()))}]");

            return sortedCards;
        }

        Debug.Log("No Three of a Kind found.");
        return new List<Card>();
    }

    private List<Card> DetermineTwoPairCards(List<Card> hand)
    {
        var groupedByValue = hand.GroupBy(card => card.Value);
        var pairs = groupedByValue.Where(group => group.Count() == 2).OrderByDescending(group => group.Key).Take(2);
        if (pairs.Count() == 2)
        {
            var remainingCard = hand.Except(pairs.SelectMany(pair => pair)).OrderByDescending(card => card.HighValue).FirstOrDefault();
            return pairs.SelectMany(pair => pair).Concat(new[] { remainingCard }).ToList();
        }
        return new List<Card>();
    }

    private List<Card> DeterminePairCards(List<Card> hand)
    {
        var groupedByValue = hand.GroupBy(card => card.Value);
        var pairGroup = groupedByValue.FirstOrDefault(group => group.Count() == 2);
        if (pairGroup != null)
        {
            var remainingCards = hand.Except(pairGroup).OrderByDescending(card => card.HighValue).Take(3);
            return pairGroup.Concat(remainingCards).ToList();
        }
        return new List<Card>();
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
        var groupedByValue = hand.Where(card => !card.IsWild).GroupBy(card => card.Value);
        int wildCount = hand.Count(card => card.IsWild);

        return groupedByValue.Any(group => group.Count() + wildCount >= 5);
    }

    private bool HasRoyalFlush(List<Card> hand)
    {
        // Sort hand by value
        var sortedHand = hand.OrderBy(card => card.Value).ToList();
        int wildCount = hand.Count(card => card.IsWild);

        // Check for flush first
        if (!HasFlush(sortedHand))
        {
            return false;
        }

        // Royal Flush values: 1 (Ace), 10, 11 (Jack), 12 (Queen), 13 (King)
        List<int> requiredValues = new List<int> { 1, 10, 11, 12, 13 };

        // Count wild cards used in the flush
        int wildsUsedInFlush = 0;

        // Track actual values in the flush
        List<int> actualValues = new List<int>();

        foreach (var card in sortedHand)
        {
            if (!card.IsWild)
            {
                actualValues.Add(card.Value);
            }
            else
            {
                wildsUsedInFlush++;
            }
        }

        // Check if all required royal flush values are present or can be filled with wild cards
        foreach (var value in actualValues)
        {
            requiredValues.Remove(value);
        }

        return requiredValues.Count <= wildsUsedInFlush;
    }


    private bool HasStraightFlush(List<Card> hand)
    {
        var wildCards = hand.Where(card => card.IsWild).ToList();
        var nonWildCards = hand.Where(card => !card.IsWild).ToList();

        var suits = nonWildCards.GroupBy(card => card.Suit);

        foreach (var suitGroup in suits)
        {
            var suitedCards = suitGroup.OrderBy(card => card.Value).ToList();
            if (HasStraightWithWilds(suitedCards, wildCards))
            {
                return true;
            }
        }

        return false;
    }

    private bool HasStraightWithWilds(List<Card> suitedCards, List<Card> wildCards)
    {
        var sortedValues = suitedCards.Select(card => card.Value).Distinct().OrderBy(value => value).ToList();
        int wildCount = wildCards.Count;

        for (int i = 0; i < sortedValues.Count - 4; i++)
        {
            int consecutiveCount = 1;
            int wildsUsed = 0;

            for (int j = i; j < sortedValues.Count - 1; j++)
            {
                if (sortedValues[j + 1] == sortedValues[j] + 1)
                {
                    consecutiveCount++;
                }
                else if (sortedValues[j + 1] != sortedValues[j])
                {
                    int gap = sortedValues[j + 1] - sortedValues[j] - 1;
                    if (wildsUsed + gap <= wildCount)
                    {
                        wildsUsed += gap;
                        consecutiveCount += gap + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (consecutiveCount + wildsUsed >= 5)
                {
                    return true;
                }
            }

            if (consecutiveCount + wildsUsed >= 5)
            {
                return true;
            }
        }

        // Check for Ace as high straight
        if (sortedValues.Contains(1) && sortedValues.Max() >= 10)
        {
            sortedValues.Add(14); // Ace as high
            sortedValues.Sort();
            return HasStraightWithWilds(sortedValues.Select(value => new Card("", value, null, null)).ToList(), wildCards);
        }

        return false;
    }

    private bool HasFourOfAKind(List<Card> hand)
    {
        var groupedByValue = hand.Where(card => !card.IsWild).GroupBy(card => card.Value);
        int wildCount = hand.Count(card => card.IsWild);

        return groupedByValue.Any(group => group.Count() + wildCount >= 4);
    }
    private bool HasFullHouse(List<Card> hand)
    {
        var groupedByValue = hand.Where(card => !card.IsWild).GroupBy(card => card.Value).ToList();
        int wildCount = hand.Count(card => card.IsWild);

        bool hasThreeOfAKind = false;
        bool hasPair = false;

        // Try to form a three of a kind first
        foreach (var group in groupedByValue)
        {
            if (group.Count() + wildCount >= 3)
            {
                hasThreeOfAKind = true;
                wildCount -= Mathf.Max(0, 3 - group.Count());  // Use wild cards for three of a kind if necessary
                groupedByValue.Remove(group);
                break;
            }
        }

        // If we formed a three of a kind, check if we can form a pair with remaining cards and wild cards
        if (hasThreeOfAKind)
        {
            foreach (var group in groupedByValue)
            {
                if (group.Count() >= 2)
                {
                    hasPair = true;
                    break;
                }
            }

            if (!hasPair && wildCount > 0)
            {
                hasPair = groupedByValue.Any(group => group.Count() == 1) || wildCount >= 2;
            }
        }

        return hasThreeOfAKind && hasPair;
    }
    private bool HasFlush(List<Card> hand)
    {
        // Separate wild cards from non-wild cards
        var nonWildCards = hand.Where(card => !card.IsWild).ToList();
        int wildCount = hand.Count(card => card.IsWild);

        // Group the non-wild cards by suit
        var groupedBySuit = nonWildCards.GroupBy(card => card.Suit);

        // Check if any suit group, with the addition of wild cards, can form a flush
        foreach (var group in groupedBySuit)
        {
            if (group.Count() + wildCount >= 5)
            {
                return true;
            }
        }

        // Special case: if the hand contains 5 wild cards, it should be considered a flush of any suit
        return wildCount >= 5;
    }
    private bool HasStraight(List<Card> hand)
    {
        var nonWildCards = hand.Where(card => !card.IsWild).OrderBy(card => card.Value).ToList();
        int wildCount = hand.Count(card => card.IsWild);

        // Function to check if there is a straight in the given list of cards
        bool HasStraightWithWilds(List<int> values, int availableWilds)
        {
            for (int i = 0; i <= values.Count - 1; i++)
            {
                int consecutiveCount = 1;
                int wildsUsed = 0;

                for (int j = i; j < values.Count - 1; j++)
                {
                    if (values[j + 1] == values[j] + 1)
                    {
                        consecutiveCount++;
                    }
                    else if (values[j + 1] != values[j])
                    {
                        int gap = values[j + 1] - values[j] - 1;
                        if (wildsUsed + gap <= availableWilds)
                        {
                            wildsUsed += gap;
                            consecutiveCount += gap + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (consecutiveCount + wildsUsed >= 6)
                {
                    return true;
                }
            }

            return false;
        }

        // Extract the values and check for a straight
        var cardValues = nonWildCards.Select(card => card.Value).Distinct().ToList();
        if (cardValues.Count < 5 - wildCount)
        {
            return false; // Not enough cards even with wilds
        }

        // Check for straight with Ace as low
        cardValues.Sort();
        if (HasStraightWithWilds(cardValues, wildCount))
        {
            return true;
        }

        // Check for straight with Ace as high
        if (cardValues.Contains(1)) // Ace present
        {
            cardValues.Add(14); // Ace as high
            cardValues.Sort();
            if (HasStraightWithWilds(cardValues, wildCount))
            {
                return true;
            }
        }

        return false;
    }

    private bool HasThreeOfAKind(List<Card> hand)
    {
        var groupedByValue = hand.Where(card => !card.IsWild).GroupBy(card => card.Value);
        int wildCount = hand.Count(card => card.IsWild);

        return groupedByValue.Any(group => group.Count() + wildCount >= 3);
    }
    private bool HasTwoPair(List<Card> hand)
    {
        var groupedByValue = hand.Where(card => !card.IsWild).GroupBy(card => card.Value);
        int wildCount = hand.Count(card => card.IsWild);

        int pairCount = groupedByValue.Count(group => group.Count() == 2);
        int singleCount = groupedByValue.Count(group => group.Count() == 1);

        return pairCount + Mathf.Min(wildCount, singleCount) >= 2;
    }
    private bool HasOnePair(List<Card> hand)
    {
        var groupedByValue = hand.Where(card => !card.IsWild).GroupBy(card => card.Value);
        int wildCount = hand.Count(card => card.IsWild);

        return groupedByValue.Any(group => group.Count() + wildCount >= 2);
    }

    public int CompareHands(List<Card> hand1, List<Card> hand2)
    {
        var result1 = EvaluateHandWeight(hand1);
        var result2 = EvaluateHandWeight(hand2);

        if (result1.rankWeight > result2.rankWeight)
        {
            Debug.Log("Player 1 wins with rank: " + result1.handRank);
            return 1; // Hand1 wins
        }
        else if (result1.rankWeight < result2.rankWeight)
        {
            Debug.Log("Player 2 wins with rank: " + result2.handRank);
            return -1; // Hand2 wins
        }
        else
        {
            int comparisonResult = CompareBestCards(result1.bestCards, result2.bestCards);

            if (comparisonResult > 0)
            {
                Debug.Log("Player 1 wins with higher cards: " + string.Join(", ", result1.bestCards.Select(card => card.ToString())));
                return 1;
            }
            else if (comparisonResult < 0)
            {
                Debug.Log("Player 2 wins with higher cards: " + string.Join(", ", result2.bestCards.Select(card => card.ToString())));
                return -1;
            }
            else
            {
                Debug.Log("It's a tie");
                return 0;
            }
        }
    }
    private int CompareBestCards(List<Card> bestCards1, List<Card> bestCards2)
    {
        Debug.Log("Comparing best cards not in for loop");
        Debug.Log($"Comparing best cards: bestCards1 count = {bestCards1.Count}, bestCards2 count = {bestCards2.Count}");
        for (int i = 0; i < bestCards1.Count; i++)
        {
            Debug.Log($"Comparing best cards in for loop: i = {i}");
            Debug.Log($"bestCards1[{i}] = {bestCards1[i].ToString()}");
            Debug.Log($"bestCards2[{i}] = {bestCards2[i].ToString()}");
            if (bestCards1[i].Value > bestCards2[i].Value)
                return 1; // Player 1 wins
            else if (bestCards1[i].Value < bestCards2[i].Value)
                return -1; // Player 2 wins
        }
        return 0;
    }
}

