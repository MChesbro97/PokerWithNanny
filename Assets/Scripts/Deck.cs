using System;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    public List<Card> cards;
    private static readonly string[] Suits = { "Club", "Diamond", "Heart", "Spade" };

    public Deck()
    {
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        cards = new List<Card>();
        foreach (string suit in Suits)
        {
            for (int value = 2; value <= 14; value++)
            {
                Sprite cardSprite = LoadCardSprite(value, suit);
                cards.Add(new Card(suit, value, cardSprite));
            }
        }
    }

    private Sprite LoadCardSprite(int value, string suit)
    {
        string path = $"PlayingCards/{suit}{value}";
        return Resources.Load<Sprite>(path);
    }

    public void Shuffle()
    {
        System.Random rng = new System.Random();
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }

    public Card Deal()
    {
        if (cards.Count == 0)
            throw new InvalidOperationException("No cards left in the deck");

        Card cardToDeal = cards[0];
        cards.RemoveAt(0);
        return cardToDeal;
    }

    public int CardsRemaining()
    {
        return cards.Count;
    }
}
