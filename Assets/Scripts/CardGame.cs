using System.Collections;
using System.Collections.Generic;
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

    private enum GameState
    {
        Idle,
        DealFirstTwoCards,
        DealNextCard,
        WaitForPlayerInput,
        DealFinalCard,
        RevealAllCards,
        CompareHands,
        GameOver
    }

    private GameState currentState;
    private int currentCardIndex;
    private List<Card> playerHand1;
    private List<Card> playerHand2;
    private List<GameObject> playerHand1Objects;
    private List<GameObject> playerHand2Objects;

    void Start()
    { 
        handEvaluator = new PokerHandEvaluator();
        //selectedGameMode = PokerGameMode.FiveCardDraw;
        NewGame();
    }

    public void NewGame()
    {
        //StopAllCoroutines();
        Debug.Log("New game started");
        foreach (GameObject card in instantiatedCards)
        {
            Destroy(card);
        }
        instantiatedCards.Clear();

        deck = new Deck();
        deck.Shuffle();
        Debug.Log("Shuffled Deck:");
        //DisplayDeck();
        Debug.Log("Dealing Cards:");

        playerHand1 = new List<Card>();
        playerHand2 = new List<Card>();
        playerHand1Objects = new List<GameObject>();
        playerHand2Objects = new List<GameObject>();

        if (selectedGameMode == PokerGameMode.FiveCardDraw)
        {
            DealFiveCardDraw();
            currentState = GameState.CompareHands;
        }
        else if (selectedGameMode == PokerGameMode.SevenCardStud)
        {
            //StartCoroutine(DealSevenCardStud());
            currentState = GameState.DealFirstTwoCards;
        }
    }
    void Update()
    {
        switch (currentState)
        {
            case GameState.DealFirstTwoCards:
                DealInitialCards();
                currentState = GameState.WaitForPlayerInput;
                break;

            case GameState.DealNextCard:
                if (currentCardIndex <= 5)
                {
                    DealNextCard(true);
                    currentState = GameState.WaitForPlayerInput;
                }
                else
                {
                    DealFinalCard(false);
                    currentState = GameState.RevealAllCards;
                }
                break;

            case GameState.WaitForPlayerInput:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentState = GameState.DealNextCard;
                }
                break;

            case GameState.RevealAllCards:
                RevealAllCards();
                currentState = GameState.CompareHands;
                break;

            case GameState.CompareHands:
                handEvaluator.CompareHands(playerHand1, playerHand2);
                currentState = GameState.GameOver;
                break;

            case GameState.GameOver:
                if (Input.GetKeyDown(KeyCode.R)) // Press R to restart the game
                {
                    NewGame();
                }
                break;
        }
    }
    private void DealFiveCardDraw()
    {
        //List<Card> playerHand1 = new List<Card>();
        //List<Card> playerHand2 = new List<Card>();

        for (int i = 0; i < 5; i++)
        {
            Card dealtCard = deck.Deal();
            Debug.Log(dealtCard);
            DisplayCard(dealtCard, i, hand1);
            playerHand1.Add(dealtCard);
        //}

        //for (int i = 0; i < 5; i++)
        //{
            dealtCard = deck.Deal();
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

    //private IEnumerator DealSevenCardStud()
    //{
    //    List<Card> playerHand1 = new List<Card>();
    //    List<Card> playerHand2 = new List<Card>();
    //    List<GameObject> playerHand1Objects = new List<GameObject>();
    //    List<GameObject> playerHand2Objects = new List<GameObject>();

    //    // Deal initial two cards face down
    //    for (int i = 0; i < 2; i++)
    //    {
    //        Card dealtCard = deck.Deal();
    //        Debug.Log(dealtCard);
    //        var cardObject = DisplayCard(dealtCard, i, hand1, faceUp: false);
    //        playerHand1.Add(dealtCard);
    //        playerHand1Objects.Add(cardObject);

    //        dealtCard = deck.Deal();
    //        Debug.Log(dealtCard);
    //        cardObject = DisplayCard(dealtCard, i, hand2, faceUp: false);
    //        playerHand2.Add(dealtCard);
    //        playerHand2Objects.Add(cardObject);
    //    }

    //    // Deal next four cards face up with waits for bets
    //    for (int i = 2; i < 6; i++)
    //    {
    //        yield return WaitForPlayerInput();

    //        Card dealtCard = deck.Deal();
    //        Debug.Log(dealtCard);
    //        var cardObject = DisplayCard(dealtCard, i, hand1, faceUp: true);
    //        playerHand1.Add(dealtCard);
    //        playerHand1Objects.Add(cardObject);

    //        //yield return WaitForPlayerInput();

    //        dealtCard = deck.Deal();
    //        Debug.Log(dealtCard);
    //        cardObject = DisplayCard(dealtCard, i, hand2, faceUp: true);
    //        playerHand2.Add(dealtCard);
    //        playerHand2Objects.Add(cardObject);
    //    }

    //    // Deal final card face down
    //    yield return WaitForPlayerInput();

    //    Card finalCard = deck.Deal();
    //    Debug.Log(finalCard);
    //    var finalCardObject = DisplayCard(finalCard, 6, hand1, faceUp: false);
    //    playerHand1.Add(finalCard);
    //    playerHand1Objects.Add(finalCardObject);

    //    finalCard = deck.Deal();
    //    Debug.Log(finalCard);
    //    finalCardObject = DisplayCard(finalCard, 6, hand2, faceUp: false);
    //    playerHand2.Add(finalCard);
    //    playerHand2Objects.Add(finalCardObject);

    //    yield return WaitForPlayerInput();

    //    TurnAllCardsFaceUp(playerHand1Objects);
    //    TurnAllCardsFaceUp(playerHand2Objects);

    //    Debug.Log($"Cards remaining in the deck: {deck.CardsRemaining()}");

    //    EvaluateHand(playerHand1, "Player 1");
    //    EvaluateHand(playerHand2, "Player 2");

    //    int comparisonResult = handEvaluator.CompareHands(playerHand1, playerHand2);
    //    if (comparisonResult > 0)
    //    {
    //        Debug.Log("Player 1 wins!");
    //    }
    //    else if (comparisonResult < 0)
    //    {
    //        Debug.Log("Player 2 wins!");
    //    }
    //    else
    //    {
    //        Debug.Log("It's a tie");
    //    }

    //    StopCoroutine(DealSevenCardStud());
    //}
    //
    //private IEnumerator WaitForPlayerInput()
    //{
    //    Debug.Log("Waiting for player input...");
    //    while (!Input.GetKeyDown(KeyCode.Space))
    //    {
    //        yield return null;
    //    }
    //    Debug.Log("Player input received.");
    //    while (Input.GetKey(KeyCode.Space))
    //    {
    //        yield return null;
    //    }
    //}
    private void DealInitialCards()
    {
        for (int i = 0; i < 2; i++)
        {
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
        currentCardIndex = 2;
    }
    private void DealNextCard(bool faceUp)
    {
        Card dealtCard = deck.Deal();
        Debug.Log(dealtCard);
        var cardObject = DisplayCard(dealtCard, currentCardIndex, hand1, faceUp);
        playerHand1.Add(dealtCard);
        playerHand1Objects.Add(cardObject);

        dealtCard = deck.Deal();
        Debug.Log(dealtCard);
        cardObject = DisplayCard(dealtCard, currentCardIndex, hand2, faceUp);
        playerHand2.Add(dealtCard);
        playerHand2Objects.Add(cardObject);

        currentCardIndex++;
    }
    private void DealFinalCard(bool faceUp)
    {
        DealNextCard(faceUp);
    }

    private void RevealAllCards()
    {
        foreach (var cardObject in playerHand1Objects)
        {
            var cardComponent = cardObject.GetComponent<CardComponent>();
            var renderer = cardObject.GetComponent<SpriteRenderer>();
            if (cardComponent != null)
            {
                renderer.sprite = cardComponent.CardData.CardSprite;
            }
        }

        foreach (var cardObject in playerHand2Objects)
        {
            var cardComponent = cardObject.GetComponent<CardComponent>();
            var renderer = cardObject.GetComponent<SpriteRenderer>();
            if (cardComponent != null)
            {
                renderer.sprite = cardComponent.CardData.CardSprite;
            }
        }

        //int comparisonResult = handEvaluator.CompareHands(playerHand1, playerHand2);
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
    //private void TurnAllCardsFaceUp(List<GameObject> cardObjects)
    //{
    //    foreach (var cardObject in cardObjects)
    //    {
    //        var cardComponent = cardObject.GetComponent<CardComponent>();
    //        var renderer = cardObject.GetComponent<SpriteRenderer>();
    //        if (cardComponent != null)
    //        {
    //            renderer.sprite = cardComponent.CardData.CardSprite;
    //        }
    //    }
    //}



    private void DisplayDeck()
    {
        foreach (Card card in deck.cards)
        {
            Debug.Log(card);
        }
    }
    private GameObject DisplayCard(Card card, int index, GameObject hand, bool faceUp = true)
    {
        Vector3 position = hand.transform.position + new Vector3(index * 2.0f, 0, 0);
        GameObject cardObject = Instantiate(cardPrefab, position, Quaternion.identity, hand.transform);
        SpriteRenderer renderer = cardObject.GetComponent<SpriteRenderer>();
        renderer.sprite = faceUp ? card.CardSprite : card.BackSprite;
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

