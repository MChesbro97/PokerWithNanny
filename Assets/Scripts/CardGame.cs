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
    public GameModeSelector gameModeSelector;

    public GameObject cardPrefab;
    public GameObject hand1;
    public GameObject hand2;

    private int sortingOrder = 0;
    private List<Card> allDealtCards = new List<Card>();
    private bool queenDealtFaceUp = false;
    void Start()
    { 
        handEvaluator = new PokerHandEvaluator();
        //selectedGameMode = PokerGameMode.FiveCardDraw;
        //NewGame();
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
        allDealtCards.Clear();
        queenDealtFaceUp = false;

        selectedGameMode = gameModeSelector.GetSelectedGameMode();

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
            case PokerGameMode.Woolworth:
            case PokerGameMode.DeucesAndJacks:
            case PokerGameMode.FollowTheQueen:
            case PokerGameMode.HighChicago:
            case PokerGameMode.LowChicago:
            case PokerGameMode.KingsAndLittleOnes:
                StartCoroutine(DealSevenCardStud());
                break;
            case PokerGameMode.NightBaseball:
                StartCoroutine(DealNightBaseball());
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
            DealAndCheckWild(playerHand1, playerHand1Objects, hand1, i, false);
            DealAndCheckWild(playerHand2, playerHand2Objects, hand2, i, false);
        }

        // Deal next four cards face up with waits for bets
        for (int i = 2; i < 6; i++)
        {
            yield return WaitForPlayerInput();
            Debug.Log("Dealing next card to player 1");
            DealAndCheckWild(playerHand1, playerHand1Objects, hand1, i, true);

            //yield return WaitForPlayerInput();
            Debug.Log("Dealing next card to player 2");
            DealAndCheckWild(playerHand2, playerHand2Objects, hand2, i, true);

        }

        // Deal final card face down
        yield return WaitForPlayerInput();

        DealAndCheckWild(playerHand1, playerHand1Objects, hand1, 6, false);
        DealAndCheckWild(playerHand2, playerHand2Objects, hand2, 6, false);

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
    private IEnumerator DealNightBaseball()
    {
        List<Card> playerHand1 = new List<Card>();
        List<Card> playerHand2 = new List<Card>();
        List<GameObject> playerHand1Objects = new List<GameObject>();
        List<GameObject> playerHand2Objects = new List<GameObject>();

        // Deal 9 cards facedown to each player
        for (int i = 0; i < 9; i++)
        {
            Debug.Log("Dealing facedown card to Player 1");
            DealAndCheckWild(playerHand1, playerHand1Objects, hand1, i, false);

            Debug.Log("Dealing facedown card to Player 2");
            DealAndCheckWild(playerHand2, playerHand2Objects, hand2, i, false);
        }

        int playerTurn = 1;
        int flipIndex1 = 8; // Start flipping from the last card dealt to Player 1
        int flipIndex2 = 8; // Start flipping from the last card dealt to Player 2

        bool player1Flipped = false;
        bool player2Flipped = false;

        while (flipIndex1 >= 0 || flipIndex2 >= 0)
        {
            if (playerTurn == 1 && flipIndex1 >= 0)
            {
                yield return WaitForPlayerInput();
                Debug.Log("Player 1 flips a card");
                FlipCard(playerHand1Objects, flipIndex1);
                flipIndex1--;
                player1Flipped = true;
                if (!player2Flipped)
                {
                    playerTurn = 2;
                }
            }
            else if (playerTurn == 2 && flipIndex2 >= 0)
            {
                yield return WaitForPlayerInput();
                Debug.Log("Player 2 flips a card");
                FlipCard(playerHand2Objects, flipIndex2);
                flipIndex2--;
                player2Flipped = true;
            }

            if (player1Flipped && player2Flipped)
            {
                // Evaluate and compare flipped cards
                var flippedCards1 = playerHand1Objects.GetRange(flipIndex1 + 1, 9 - (flipIndex1 + 1)).Select(go => go.GetComponent<CardComponent>().CardData).ToList();
                var flippedCards2 = playerHand2Objects.GetRange(flipIndex2 + 1, 9 - (flipIndex2 + 1)).Select(go => go.GetComponent<CardComponent>().CardData).ToList();

                int comparisonResult = handEvaluator.CompareHands(flippedCards1, flippedCards2);
                if (comparisonResult > 0)
                {
                    Debug.Log("Player 1 is winning currently.");
                }
                else if (comparisonResult < 0)
                {
                    Debug.Log("Player 2 is winning currently.");
                }
                else
                {
                    Debug.Log("Both players are tied.");
                }

                if (comparisonResult <= 0 && playerTurn == 1)
                {
                    playerTurn = 1; 
                }
                else if (comparisonResult > 0 && playerTurn == 1)
                {
                    playerTurn = 2; 
                }
                else if (comparisonResult >= 0 && playerTurn ==2)
                {
                    playerTurn = 2;
                }
                else if (comparisonResult < 0 && playerTurn == 2)
                {
                    playerTurn = 1;
                }
            }
            
        }

        // Final evaluation
        var finalFlippedCards1 = playerHand1Objects.GetRange(flipIndex1 + 1, 9 - (flipIndex1 + 1)).Select(go => go.GetComponent<CardComponent>().CardData).ToList();
        var finalFlippedCards2 = playerHand2Objects.GetRange(flipIndex2 + 1, 9 - (flipIndex2 + 1)).Select(go => go.GetComponent<CardComponent>().CardData).ToList();

        int finalComparisonResult = handEvaluator.CompareHands(finalFlippedCards1, finalFlippedCards2);
        if (finalComparisonResult > 0)
        {
            Debug.Log("Player 1 wins the game!");
        }
        else if (finalComparisonResult < 0)
        {
            Debug.Log("Player 2 wins the game!");
        }
        else
        {
            Debug.Log("It's a tie!");
        }
    }

    private void FlipCard(List<GameObject> handObjects, int index)
    {
        if (index >= 0 && index < handObjects.Count)
        {
            var cardComponent = handObjects[index].GetComponent<CardComponent>();
            var renderer = handObjects[index].GetComponent<SpriteRenderer>();
            if (cardComponent != null && renderer != null)
            {
                renderer.sprite = cardComponent.CardData.CardSprite;
            }
        }
    }


    private void DealAndCheckWild(List<Card> playerHand, List<GameObject> playerHandObjects, GameObject hand, int index, bool faceUp)
    {
        Card dealtCard = deck.Deal();
        Debug.Log(dealtCard);
        var cardObject = DisplayCard(dealtCard, index, hand, faceUp);
        playerHand.Add(dealtCard);
        playerHandObjects.Add(cardObject);
        allDealtCards.Add(dealtCard);

        if (selectedGameMode == PokerGameMode.FollowTheQueen)
        {
            if (dealtCard.Value == 12 && faceUp) // Queen is dealt face up
            {
                Debug.Log("Queen dealt face up, setting next card as wild");
                queenDealtFaceUp = true;
                Card.SetFollowTheQueenWildValue(-1);
                UpdateAllCardsWildStatus();// Reset wild value
            }
            else if (queenDealtFaceUp && Card.GetFollowTheQueenWildValue() == -1 && faceUp) // This card is dealt after a face-up queen
            {
                Card.SetFollowTheQueenWildValue(dealtCard.Value);
                Debug.Log($"New wild card value set to: {dealtCard.Value}");
                UpdateAllCardsWildStatus();
                queenDealtFaceUp=false;
            }
        }
    }

    private void UpdateAllCardsWildStatus()
    {
        deck.UpdateAllCardsWildStatus(); // Update undealt cards

        foreach (var card in allDealtCards)
        {
            card.SetWildStatus(); // Update dealt cards
        }
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

