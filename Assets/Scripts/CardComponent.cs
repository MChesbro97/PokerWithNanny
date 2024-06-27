using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardComponent : MonoBehaviour
{
    public Card CardData { get; private set; }

    public void Initialize(Card card)
    {
        CardData = card;
    }
}
