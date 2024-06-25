using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string Suit { get; private set; }
    public int Value { get; private set; }
    public Sprite CardSprite { get; private set; }

    public Card(string suit, int value, Sprite cardSprite)
    {
        Suit = suit;
        Value = value;
        CardSprite = cardSprite;
    }

    public override string ToString()
    {
        return $"{Value} of {Suit}";
    }
}
