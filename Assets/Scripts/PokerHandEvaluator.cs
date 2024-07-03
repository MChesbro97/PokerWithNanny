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
        Debug.Log($"Determining Five of a Kind cards. Hand: [{string.Join(", ", hand.Select(card => card.ToString()))}]");

        var groupedByValue = hand.Where(card => !card.IsWild).GroupBy(card => card.Value);
        int wildCount = hand.Count(card => card.IsWild);

        // Find the group with the highest count + wild cards that form five of a kind
        var fiveOfAKindGroup = groupedByValue
            .Where(group => group.Count() + wildCount >= 5)
            .OrderByDescending(group => group.Key)
            .FirstOrDefault();

        if (fiveOfAKindGroup != null)
        {
            Debug.Log($"Found potential Five of a Kind group: [{string.Join(", ", fiveOfAKindGroup.Select(card => card.ToString()))}]");

            // Determine how many wild cards are needed to complete the five of a kind
            wildCount = 5 - fiveOfAKindGroup.Count();
            var actualFiveOfAKind = fiveOfAKindGroup.ToList();

            // Add wild cards if necessary
            if (wildCount > 0)
            {
                var wildCards = hand.Where(card => card.IsWild).Take(wildCount);
                actualFiveOfAKind.AddRange(wildCards);
                Debug.Log($"Added {wildCount} wild card(s): [{string.Join(", ", wildCards.Select(card => card.ToString()))}]");
            }

            // Sort the actualFiveOfAKind by natural cards first, then wild cards
            actualFiveOfAKind = actualFiveOfAKind
                .OrderByDescending(card => !card.IsWild)  // Natural cards first
                .ThenByDescending(card => card.Value)     // Then by value
                .ToList();

            Debug.Log($"Sorted Five of a Kind cards: [{string.Join(", ", actualFiveOfAKind.Select(card => card.ToString()))}]");

            return actualFiveOfAKind;
        }

        Debug.Log("No Five of a Kind found.");
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
        Debug.Log($"Determining Four of a Kind cards. Hand: [{string.Join(", ", hand.Select(card => card.ToString()))}]");

        var groupedByValue = hand.Where(card => !card.IsWild).GroupBy(card => card.Value);
        int wildCount = hand.Count(card => card.IsWild);

        // Find the group with the highest count + wild cards that form four of a kind
        var fourOfAKindGroup = groupedByValue
            .Where(group => group.Count() + wildCount >= 4)
            .OrderByDescending(group => group.Key)
            .FirstOrDefault();

        if (fourOfAKindGroup != null)
        {
            Debug.Log($"Found potential Four of a Kind group: [{string.Join(", ", fourOfAKindGroup.Select(card => card.ToString()))}]");

            // Determine how many wild cards are needed to complete the four of a kind
            wildCount = 4 - fourOfAKindGroup.Count();
            var actualFourOfAKind = fourOfAKindGroup.ToList();

            // Add wild cards if necessary
            if (wildCount > 0)
            {
                var wildCards = hand.Where(card => card.IsWild).Take(wildCount);
                actualFourOfAKind.AddRange(wildCards);
                Debug.Log($"Added {wildCount} wild card(s): [{string.Join(", ", wildCards.Select(card => card.ToString()))}]");
            }

            // Determine the kicker(s) for the four of a kind
            var kickers = hand.Except(actualFourOfAKind).OrderByDescending(card => card.Value).Take(1).ToList();
            Debug.Log($"Kicker: [{string.Join(", ", kickers.Select(card => card.ToString()))}]");

            // Sort the actualFourOfAKind by natural cards first, then wild cards
            actualFourOfAKind = actualFourOfAKind
                .OrderByDescending(card => !card.IsWild)  // Natural cards first
                .ThenByDescending(card => card.Value)     // Then by value
                .ToList();

            Debug.Log($"Sorted Four of a Kind cards: [{string.Join(", ", actualFourOfAKind.Select(card => card.ToString()))}]");

            // Combine the Four of a Kind with the kicker
            var finalHand = actualFourOfAKind.Concat(kickers).ToList();
            Debug.Log($"Final hand: [{string.Join(", ", finalHand.Select(card => card.ToString()))}]");

            return finalHand;
        }

        Debug.Log("No Four of a Kind found.");
        return new List<Card>();
    }


    private List<Card> DetermineFullHouseCards(List<Card> hand)
    {
        Debug.Log($"Determining Full House cards. Hand: [{string.Join(", ", hand.Select(card => card.ToString()))}]");

        var groupedByValue = hand.Where(card => !card.IsWild).GroupBy(card => card.Value).ToList();
        int wildCount = hand.Count(card => card.IsWild);

        // Find the group with the highest count + wild cards that form three of a kind
        var threeOfAKindGroup = groupedByValue
            .Where(group => group.Count() + wildCount >= 3)
            .OrderByDescending(group => group.Key)
            .FirstOrDefault();

        if (threeOfAKindGroup != null)
        {
            // Adjust wild count
            wildCount -= 3 - threeOfAKindGroup.Count();
            var actualThreeOfAKind = threeOfAKindGroup.ToList();

            // Add wild cards if necessary
            if (wildCount > 0)
            {
                var wildCards = hand.Where(card => card.IsWild).Take(3 - actualThreeOfAKind.Count());
                actualThreeOfAKind.AddRange(wildCards);
                Debug.Log($"Added {wildCards.Count()} wild card(s) to complete Three of a Kind: [{string.Join(", ", wildCards.Select(card => card.ToString()))}]");
            }

            // Exclude the three of a kind cards and find a pair
            var remainingCards = hand.Except(actualThreeOfAKind).ToList();
            groupedByValue = remainingCards.Where(card => !card.IsWild).GroupBy(card => card.Value).ToList();

            var pairGroup = groupedByValue
                .Where(group => group.Count() + wildCount >= 2)
                .OrderByDescending(group => group.Key)
                .FirstOrDefault();

            if (pairGroup != null)
            {
                var actualPair = pairGroup.ToList();

                // Add wild cards if necessary
                if (wildCount > 0)
                {
                    var wildCards = remainingCards.Where(card => card.IsWild).Take(2 - actualPair.Count());
                    actualPair.AddRange(wildCards);
                    Debug.Log($"Added {wildCards.Count()} wild card(s) to complete Pair: [{string.Join(", ", wildCards.Select(card => card.ToString()))}]");
                }

                // Sort the actualThreeOfAKind and actualPair by natural cards first, then wild cards
                actualThreeOfAKind = actualThreeOfAKind
                    .OrderByDescending(card => !card.IsWild)  // Natural cards first
                    .ThenByDescending(card => card.Value)     // Then by value
                    .ToList();

                actualPair = actualPair
                    .OrderByDescending(card => !card.IsWild)  // Natural cards first
                    .ThenByDescending(card => card.Value)     // Then by value
                    .ToList();

                var finalHand = actualThreeOfAKind.Concat(actualPair).ToList();
                Debug.Log($"Final hand: [{string.Join(", ", finalHand.Select(card => card.ToString()))}]");

                return finalHand;
            }
        }

        Debug.Log("No Full House found.");
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

        var groupedByValue = hand.Where(card => !card.IsWild).GroupBy(card => card.Value);
        int wildCount = hand.Count(card => card.IsWild);

        // Find the group with the highest count + wild cards that form three of a kind
        var threeOfAKindGroup = groupedByValue
            .Where(group => group.Count() + wildCount >= 3)
            .OrderByDescending(group => group.Key)
            .FirstOrDefault();

        if (threeOfAKindGroup != null)
        {
            Debug.Log($"Found potential Three of a Kind group: [{string.Join(", ", threeOfAKindGroup.Select(card => card.ToString()))}]");

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

            // Include the remaining cards in the hand excluding the Three of a Kind cards
            var remainingCards = hand.Except(actualThreeOfAKind).OrderByDescending(card => card.Value).ToList();
            Debug.Log($"Remaining cards: [{string.Join(", ", remainingCards.Select(card => card.ToString()))}]");

            // Sort the actualThreeOfAKind by natural cards first, then wild cards
            actualThreeOfAKind = actualThreeOfAKind
                .OrderByDescending(card => !card.IsWild)  // Natural cards first
                .ThenByDescending(card => card.Value)     // Then by value
                .ToList();

            Debug.Log($"Sorted Three of a Kind cards: [{string.Join(", ", actualThreeOfAKind.Select(card => card.ToString()))}]");

            // Combine the Three of a Kind with the remaining highest cards
            var finalHand = actualThreeOfAKind.Concat(remainingCards.Take(2)).ToList();
            Debug.Log($"Final hand: [{string.Join(", ", finalHand.Select(card => card.ToString()))}]");

            return finalHand;
        }

        Debug.Log("No Three of a Kind found.");
        return new List<Card>();
    }

    private List<Card> DetermineTwoPairCards(List<Card> hand)
    {
        Debug.Log($"Determining Two Pair cards. Hand: [{string.Join(", ", hand.Select(card => card.ToString()))}]");

        var groupedByValue = hand.Where(card => !card.IsWild).GroupBy(card => card.Value).ToList();
        int wildCount = hand.Count(card => card.IsWild);

        // Find pairs
        var pairs = groupedByValue.Where(group => group.Count() == 2).OrderByDescending(group => group.Key).ToList();

        // If we have two natural pairs
        if (pairs.Count >= 2)
        {
            var topTwoPairs = pairs.Take(2).SelectMany(group => group).ToList();
            var remainingCards = hand.Except(topTwoPairs).OrderByDescending(card => card.Value).Take(1).ToList();
            var finalHand = topTwoPairs.Concat(remainingCards).ToList();
            Debug.Log($"Final hand with natural pairs: [{string.Join(", ", finalHand.Select(card => card.ToString()))}]");
            return finalHand;
        }

        // If we need to use wild cards to complete pairs
        List<Card> bestTwoPairs = new List<Card>();
        foreach (var group in groupedByValue)
        {
            if (group.Count() == 1 && wildCount > 0)
            {
                wildCount--;
                bestTwoPairs.AddRange(group.Concat(hand.Where(card => card.IsWild).Take(1)));
                if (bestTwoPairs.Count == 4)
                    break;
            }
        }

        if (bestTwoPairs.Count < 4 && wildCount > 0)
        {
            var highestPair = pairs.FirstOrDefault();
            if (highestPair != null)
            {
                bestTwoPairs.AddRange(highestPair);
                wildCount--;
                var wildCard = hand.Where(card => card.IsWild).FirstOrDefault();
                if (wildCard != null)
                {
                    bestTwoPairs.Add(wildCard);
                }
            }
        }

        if (bestTwoPairs.Count == 4)
        {
            var remainingCards = hand.Except(bestTwoPairs).OrderByDescending(card => card.Value).Take(1).ToList();
            var finalHand = bestTwoPairs.Concat(remainingCards).ToList();
            Debug.Log($"Final hand with wild cards: [{string.Join(", ", finalHand.Select(card => card.ToString()))}]");
            return finalHand;
        }

        Debug.Log("No Two Pair found.");
        return new List<Card>();
    }


    private List<Card> DeterminePairCards(List<Card> hand)
    {
        Debug.Log($"Determining Pair cards. Hand: [{string.Join(", ", hand.Select(card => card.ToString()))}]");

        var groupedByValue = hand.Where(card => !card.IsWild).GroupBy(card => card.Value).ToList();
        int wildCount = hand.Count(card => card.IsWild);

        // Find pairs
        var pairs = groupedByValue.Where(group => group.Count() == 2).OrderByDescending(group => group.Key).ToList();

        // If we have a natural pair
        if (pairs.Count > 0)
        {
            var topPair = pairs.First().ToList();
            var remainingCards = hand.Except(topPair).OrderByDescending(card => card.Value).Take(3).ToList();
            var finalHand = topPair.Concat(remainingCards).ToList();
            Debug.Log($"Final hand with natural pair: [{string.Join(", ", finalHand.Select(card => card.ToString()))}]");
            return finalHand;
        }

        // If we need to use wild cards to complete a pair
        var singleCards = groupedByValue.Where(group => group.Count() == 1).OrderByDescending(group => group.Key).ToList();
        if (singleCards.Count > 0 && wildCount > 0)
        {
            var topSingle = singleCards.First().ToList();
            wildCount--;
            var wildCard = hand.Where(card => card.IsWild).FirstOrDefault();
            var pairWithWild = topSingle.Concat(new[] { wildCard }).ToList();
            var remainingCards = hand.Except(pairWithWild).OrderByDescending(card => card.Value).Take(3).ToList();
            var finalHand = pairWithWild.Concat(remainingCards).ToList();
            Debug.Log($"Final hand with wild card: [{string.Join(", ", finalHand.Select(card => card.ToString()))}]");
            return finalHand;
        }

        Debug.Log("No Pair found.");
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

        // Group by suit to check for flush
        var suits = sortedHand.GroupBy(card => card.Suit);

        foreach (var suitGroup in suits)
        {
            // Ensure we have at least one non-wild card in the suit group
            if (suitGroup.Count(card => !card.IsWild) > 0)
            {
                var suitedCards = suitGroup.Where(card => !card.IsWild).OrderBy(card => card.Value).ToList();
                var wildCards = suitGroup.Where(card => card.IsWild).ToList();

                List<int> requiredValues = new List<int> { 10, 11, 12, 13, 14 };
                int wildsUsed = 0;

                foreach (var card in suitedCards)
                {
                    if (requiredValues.Contains(card.Value))
                    {
                        requiredValues.Remove(card.Value);
                    }
                }

                wildsUsed = requiredValues.Count;
                if (wildsUsed <= wildCards.Count)
                {
                    return true;
                }
            }
        }

        return false;
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

