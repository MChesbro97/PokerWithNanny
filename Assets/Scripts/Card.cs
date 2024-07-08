using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Card
{
    public string Suit { get; private set; }
    public int Value { get; private set; }
    public Sprite CardSprite { get; private set; }
    public Sprite BackSprite { get; private set; }

    public bool IsWild { get; private set; }

    private static PokerGameMode currentGameMode;
    private static int followTheQueenWildValue = -1;
    private static List<Card> currentHand;

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
        followTheQueenWildValue = -1;
    }
    public static void SetCurrentHand(List<Card> hand)
    {
        currentHand = hand;
        UpdateAllCardsWildStatus();
    }
    public static void SetFollowTheQueenWildValue(int value)
    {
        followTheQueenWildValue = value;
    }
    public static int GetFollowTheQueenWildValue()
    {
        return followTheQueenWildValue;
    }
    public void SetWildStatus()
    {
        switch (currentGameMode)
        {
            case PokerGameMode.FiveCardDraw:
                IsWild = false;
                break;
            case PokerGameMode.DayBaseball:
            case PokerGameMode.NightBaseball:
                IsWild = (Value == 3 || Value == 9);
                break;
            case PokerGameMode.Woolworth:
                IsWild = (Value == 5 || Value == 10);
                break;
            case PokerGameMode.DeucesAndJacks:
                IsWild = (Value == 2 || Value == 11 || (Value == 13 && Suit == "Diamond"));
                break;
            case PokerGameMode.FollowTheQueen:
                IsWild = (Value == 12 || Value == followTheQueenWildValue);
                break;
            case PokerGameMode.KingsAndLittleOnes:
                IsWild = (Value == 13 || IsLowestValueInHand());
                break;
            default:
                IsWild = false;
                break;
        }
        Debug.Log($"Card {this} wild status: {IsWild}");
    }
    private bool IsLowestValueInHand()
    {
        if (currentGameMode != PokerGameMode.KingsAndLittleOnes || currentHand == null)
        {
            return false;
        }

        int lowestValue = currentHand.Min(card => card.Value);
        return Value == lowestValue;
    }
    public static void UpdateAllCardsWildStatus()
    {
        if (currentHand != null)
        {
            foreach (var card in currentHand)
            {
                card.SetWildStatus();
            }
        }
    }
}
