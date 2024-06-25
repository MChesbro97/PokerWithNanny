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

        if(handRank == PokerHandEvaluator.HandRank.OnePair)
        {
            Debug.Log($"{playerName} has a pair");
        }
        if (handRank == PokerHandEvaluator.HandRank.TwoPair)
        {
            Debug.Log($"{playerName} has two pairs");
        }
        if (handRank == PokerHandEvaluator.HandRank.ThreeOfAKind)
        {
            Debug.Log($"{playerName} has three of a kind");
        }
        if (handRank == PokerHandEvaluator.HandRank.Straight)
        {
            Debug.Log($"{playerName} has a straight");
        }
        if (handRank == PokerHandEvaluator.HandRank.Flush)
        {
            Debug.Log($"{playerName} has a flush");
        }
        if (handRank == PokerHandEvaluator.HandRank.FullHouse)
        {
            Debug.Log($"{playerName} has a full house");
        }
        if (handRank == PokerHandEvaluator.HandRank.FourOfAKind)
        {
            Debug.Log($"{playerName} has four of a kind");
        }
        if (handRank == PokerHandEvaluator.HandRank.StraightFlush)
        {
            Debug.Log($"{playerName} has a straight flush");
        }
        if (handRank == PokerHandEvaluator.HandRank.RoyalFlush)
        {
            Debug.Log($"{playerName} has a Royal Flush");
        }
        if (handRank == PokerHandEvaluator.HandRank.FiveOfAKind)
        {
            Debug.Log($"{playerName} has five of a kind");
        }
    }
}
