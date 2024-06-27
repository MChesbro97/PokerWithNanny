using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string Suit { get; private set; }
    public int Value { get; private set; }
    public Sprite CardSprite { get; private set; }
    public Sprite BackSprite { get; private set; }

    public Card(string suit, int value, Sprite cardSprite, Sprite backSprite)
    {
        Suit = suit;
        Value = value;
        CardSprite = cardSprite;
        BackSprite = backSprite;
    }

    public int HighValue
    {
        get { return Value == 1 ? 14 : Value; }
    }

    public override string ToString()
    {
        return $"{Value} of {Suit}";
    }
}
