using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string Suit { get; private set; }
    public int Value { get; private set; }
    public Sprite CardSprite { get; private set; }
    public Sprite BackSprite { get; private set; }

    public bool IsWild { get; private set; }

    private static PokerGameMode currentGameMode;

    public Card(string suit, int value, Sprite cardSprite, Sprite backSprite)
    {
        Suit = suit;
        Value = value;
        CardSprite = cardSprite;
        BackSprite = backSprite;
        SetWildStatus();
    }

    public int HighValue
    {
        get { return Value == 1 ? 14 : Value; }
    }

    public override string ToString()
    {
        return $"{Value} of {Suit}";
    }
    public static void SetGameMode(PokerGameMode gameMode)
    {
        currentGameMode = gameMode;
        //UpdateAllCardsWildStatus();
    }
    public void SetWildStatus()
    {
        switch (currentGameMode)
        {
            case PokerGameMode.FiveCardDraw:
                IsWild = false;
                break;
            case PokerGameMode.DayBaseball:
                IsWild = (Value == 3 || Value == 9);
                break;
            case PokerGameMode.Woolworth:
                IsWild = (Value == 5 || Value == 10);
                break;
            default:
                IsWild = false;
                break;
        }
        Debug.Log($"Card {this} wild status: {IsWild}");
    }

    //public static void UpdateAllCardsWildStatus()
    //{
    //    // Assuming all cards are stored somewhere, update their wild status
    //    // This method should be called after the game mode is set to update existing cards
    //    var allCards = FindObjectsOfType<Card>(); // Example, adjust based on your implementation
    //    foreach (var cardComponent in allCards)
    //    {
    //        cardComponent.CardData.SetWildStatus();
    //    }
    //}
}
