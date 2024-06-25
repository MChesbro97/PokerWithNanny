using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class CardGame : MonoBehaviour
{
    private Deck deck;
    private List<GameObject> instantiatedCards = new List<GameObject>();
    private PokerHandEvaluator handEvaluator;

    public GameObject cardPrefab;
    public GameObject hand1;
    public GameObject hand2;

    void Start()
    { 
        handEvaluator = new PokerHandEvaluator();

        NewGame();
    }

    public void NewGame()
    {
        foreach(GameObject card in instantiatedCards)
        {
            Destroy(card);
        }
        instantiatedCards.Clear();

        deck = new Deck();
        deck.Shuffle();
        Debug.Log("Shuffled Deck:");
        DisplayDeck();
        Debug.Log("Dealing Cards:");

        List<Card> playerHand1 = new List<Card>();
        List<Card> playerHand2 = new List<Card>();

        for (int i = 0; i < 5; i++)
        {
            Card dealtCard = deck.Deal();
            Debug.Log(dealtCard);
            DisplayCard(dealtCard, i, hand1);
            playerHand1.Add(dealtCard);
        }

        for (int i = 0; i < 5; i++)
        {
            Card dealtCard = deck.Deal();
            Debug.Log(dealtCard);
            DisplayCard(dealtCard, i, hand2);
            playerHand2.Add(dealtCard);
        }

        Debug.Log($"Cards remaining in the deck: {deck.CardsRemaining()}");

        EvaluateHand(playerHand1, "Player 1");
        EvaluateHand(playerHand2, "Player 2");

        CompareHands(playerHand1, playerHand2);
    }
    private void DisplayDeck()
    {
        foreach (Card card in deck.cards)
        {
            Debug.Log(card);
        }
    }
    private void DisplayCard(Card card, int index, GameObject hand)
    {
        Vector3 position = hand.transform.position + new Vector3(index * 2.0f, 0, 0);
        GameObject cardObject = Instantiate(cardPrefab, position, Quaternion.identity, hand.transform);
        SpriteRenderer renderer = cardObject.GetComponent<SpriteRenderer>();
        renderer.sprite = card.CardSprite;
        instantiatedCards.Add(cardObject);
    }

    private void EvaluateHand(List<Card> hand, string playerName)
    {
        PokerHandEvaluator.HandRank handRank = handEvaluator.EvaluateHand(hand);
        switch (handRank)
        {
            case PokerHandEvaluator.HandRank.OnePair:
                Debug.Log($"{playerName} has a pair");
                break;
            case PokerHandEvaluator.HandRank.TwoPair:
                Debug.Log($"{playerName} has two pairs");
                break;
            case PokerHandEvaluator.HandRank.ThreeOfAKind:
                Debug.Log($"{playerName} has three of a kind");
                break;
            case PokerHandEvaluator.HandRank.Straight:
                Debug.Log($"{playerName} has a straight");
                break;
            case PokerHandEvaluator.HandRank.Flush:
                Debug.Log($"{playerName} has a flush");
                break;
            case PokerHandEvaluator.HandRank.FullHouse:
                Debug.Log($"{playerName} has a full house");
                break;
            case PokerHandEvaluator.HandRank.FourOfAKind:
                Debug.Log($"{playerName} has four of a kind");
                break;
            case PokerHandEvaluator.HandRank.StraightFlush:
                Debug.Log($"{playerName} has a straight flush");
                break;
            case PokerHandEvaluator.HandRank.RoyalFlush:
                Debug.Log($"{playerName} has a Royal Flush");
                break;
            case PokerHandEvaluator.HandRank.FiveOfAKind:
                Debug.Log($"{playerName} has five of a kind");
                break;
            default:
                Debug.Log($"{playerName} has {handRank}");
                break;
        }
    }

    private void CompareHands(List<Card> hand1, List<Card> hand2)
    {
        var result1 = handEvaluator.EvaluateHandWeight(hand1);
        var result2 = handEvaluator.EvaluateHandWeight(hand2);

        if (result1.rankWeight > result2.rankWeight)
        {
            Debug.Log("Player 1 wins!");
        }
        else if (result1.rankWeight < result2.rankWeight)
        {
            Debug.Log("Player 2 wins!");
        }
        else
        {
            int comparisonResult = CompareBestCards(result1.bestCards, result2.bestCards);

            if(comparisonResult > 0)
            {
                Debug.Log("Player 1 wins with higher cards");
            }
            else if(comparisonResult < 0)
            {
                Debug.Log("Player 2 wins with higher cards");
            }
            else
            {
                Debug.Log("It's a tie");
            }
        }
    }

    private int CompareBestCards(List<Card> bestCards1, List<Card> bestCards2)
    {

        return 0;
    }
}
