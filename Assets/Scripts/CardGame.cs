using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class CardGame : MonoBehaviour
{
    private Deck deck;
    private List<GameObject> instantiatedCards = new List<GameObject>();
    private PokerHandEvaluator handEvaluator;
    public PokerGameMode selectedGameMode;

    public GameObject cardPrefab;
    public GameObject hand1;
    public GameObject hand2;

    private int sortingOrder = 0;
    void Start()
    { 
        handEvaluator = new PokerHandEvaluator();
        //selectedGameMode = PokerGameMode.FiveCardDraw;
        NewGame();
    }

    public void NewGame()
    {
        Debug.Log("New game started");
        StopAllCoroutines();

        foreach (GameObject card in instantiatedCards)
        {
            Destroy(card);
        }
        instantiatedCards.Clear();
        sortingOrder = 0;

        Card.SetGameMode(selectedGameMode);

        deck = new Deck();
        deck.Shuffle();
        Debug.Log("Shuffled Deck:");
        //DisplayDeck();
        Debug.Log("Dealing Cards:");



        switch (selectedGameMode)
        {
            case PokerGameMode.FiveCardDraw:
                DealFiveCardDraw();
                break;

            case PokerGameMode.DayBaseball:
                //StartCoroutine(DealSevenCardStud());
                //break;
            case PokerGameMode.Woolworth:
                StartCoroutine(DealSevenCardStud());
                break;

            default:
                Debug.LogError("Unknown game mode selected!");
                break;
        }
    }

    private void DealFiveCardDraw()
    {
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

        int comparisonResult = handEvaluator.CompareHands(playerHand1, playerHand2);
        if (comparisonResult > 0)
        {
            Debug.Log("Player 1 wins!");
        }
        else if (comparisonResult < 0)
        {
            Debug.Log("Player 2 wins!");
        }
        else
        {
            Debug.Log("It's a tie");
        }
    }

    private IEnumerator DealSevenCardStud()
    {
        List<Card> playerHand1 = new List<Card>();
        List<Card> playerHand2 = new List<Card>();
        List<GameObject> playerHand1Objects = new List<GameObject>();
        List<GameObject> playerHand2Objects = new List<GameObject>();

        // Deal initial two cards face down
        for (int i = 0; i < 2; i++)
        {
            Debug.Log("First 2 cards being dealt facedown");
            Card dealtCard = deck.Deal();
            Debug.Log(dealtCard);
            var cardObject = DisplayCard(dealtCard, i, hand1, faceUp: false);
            playerHand1.Add(dealtCard);
            playerHand1Objects.Add(cardObject);

            dealtCard = deck.Deal();
            Debug.Log(dealtCard);
            cardObject = DisplayCard(dealtCard, i, hand2, faceUp: false);
            playerHand2.Add(dealtCard);
            playerHand2Objects.Add(cardObject);
        }

        // Deal next four cards face up with waits for bets
        for (int i = 2; i < 6; i++)
        {
            yield return WaitForPlayerInput();
            Debug.Log("Dealing next card to player 1");
            Card dealtCard = deck.Deal();
            Debug.Log(dealtCard);
            var cardObject = DisplayCard(dealtCard, i, hand1, faceUp: true);
            playerHand1.Add(dealtCard);
            playerHand1Objects.Add(cardObject);

            //yield return WaitForPlayerInput();
            Debug.Log("Dealing next card to player 2");
            dealtCard = deck.Deal();
            Debug.Log(dealtCard);
            cardObject = DisplayCard(dealtCard, i, hand2, faceUp: true);
            playerHand2.Add(dealtCard);
            playerHand2Objects.Add(cardObject);
        }

        // Deal final card face down
        yield return WaitForPlayerInput();

        Card finalCard = deck.Deal();
        Debug.Log(finalCard);
        var finalCardObject = DisplayCard(finalCard, 6, hand1, faceUp: false);
        playerHand1.Add(finalCard);
        playerHand1Objects.Add(finalCardObject);

        finalCard = deck.Deal();
        Debug.Log(finalCard);
        finalCardObject = DisplayCard(finalCard, 6, hand2, faceUp: false);
        playerHand2.Add(finalCard);
        playerHand2Objects.Add(finalCardObject);

        yield return WaitForPlayerInput();

        TurnAllCardsFaceUp(playerHand1Objects);
        TurnAllCardsFaceUp(playerHand2Objects);

        Debug.Log($"Cards remaining in the deck: {deck.CardsRemaining()}");

        EvaluateHand(playerHand1, "Player 1");
        EvaluateHand(playerHand2, "Player 2");

        //string playerHand1String = string.Join(", ", playerHand1.Select(card => $"{card.Value} of {card.Suit}"));
        //string playerHand2String = string.Join(", ", playerHand2.Select(card => $"{card.Value} of {card.Suit}"));

        //// Log the hands
        //Debug.Log($"Player 1's hand: [{playerHand1String}]");
        //Debug.Log($"Player 2's hand: [{playerHand2String}]");

        int comparisonResult = handEvaluator.CompareHands(playerHand1, playerHand2);
        //if (comparisonResult > 0)
        //{
        //    Debug.Log("Player 1 wins!");
        //}
        //else if (comparisonResult < 0)
        //{
        //    Debug.Log("Player 2 wins!");
        //}
        //else
        //{
        //    Debug.Log("It's a tie");
        //}
    }
    private void TurnAllCardsFaceUp(List<GameObject> cardObjects)
    {
        foreach (var cardObject in cardObjects)
        {
            var cardComponent = cardObject.GetComponent<CardComponent>();
            var renderer = cardObject.GetComponent<SpriteRenderer>();
            if (cardComponent != null)
            {
                renderer.sprite = cardComponent.CardData.CardSprite;
            }
        }
    }

    private IEnumerator WaitForPlayerInput()
    {
        Debug.Log("Waiting for player input...");
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        Debug.Log("Player input received.");
        while (Input.GetKey(KeyCode.Space))
        {
            yield return null;
        }
    }

    private void DisplayDeck()
    {
        foreach (Card card in deck.cards)
        {
            Debug.Log(card);
        }
    }
    private GameObject DisplayCard(Card card, int index, GameObject hand, bool faceUp = true)
    {
        Vector3 position = hand.transform.position + new Vector3(index * 1.0f, 0, 0);
        GameObject cardObject = Instantiate(cardPrefab, position, Quaternion.identity, hand.transform);
        SpriteRenderer renderer = cardObject.GetComponent<SpriteRenderer>();
        renderer.sprite = faceUp ? card.CardSprite : card.BackSprite;
        renderer.sortingOrder = sortingOrder++;
        var cardComponent = cardObject.AddComponent<CardComponent>();
        cardComponent.Initialize(card);
        instantiatedCards.Add(cardObject);
        return cardObject;
    }

    private void EvaluateHand(List<Card> hand, string playerName)
    {
        var result = handEvaluator.EvaluateHandWeight(hand);
        Debug.Log($"{playerName} has a {result.handRank}");
        
    }

    //private int CompareBestCards(List<Card> bestCards1, List<Card> bestCards2)
    //{
    //    for (int i = 0; i < bestCards1.Count; i++)
    //    {
    //        if (bestCards1[i].Value > bestCards2[i].Value)
    //            return 1; // Player 1 wins
    //        else if (bestCards1[i].Value < bestCards2[i].Value)
    //            return -1; // Player 2 wins
    //    }
    //    return 0;
    //}
}

