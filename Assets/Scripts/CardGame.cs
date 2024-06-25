using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGame : MonoBehaviour
{
    private Deck deck;
    public GameObject cardPrefab;

    void Start()
    {
        deck = new Deck();
        deck.Shuffle();
        Debug.Log("Shuffled Deck:");
        DisplayDeck();
        Debug.Log("Dealing Cards:");
        for (int i = 0; i < 5; i++)
        {
            Card dealtCard = deck.Deal();
            Debug.Log(dealtCard);
            DisplayCard(dealtCard, new Vector3(i * 2.0f, 0, 0));
        }

        Debug.Log($"Cards remaining in the deck: {deck.CardsRemaining()}");
    }

    private void DisplayDeck()
    {
        foreach (Card card in deck.cards)
        {
            Debug.Log(card);
        }
    }
    private void DisplayCard(Card card, Vector3 position)
    {
        GameObject cardObject = Instantiate(cardPrefab, position, Quaternion.identity);
        SpriteRenderer renderer = cardObject.GetComponent<SpriteRenderer>();
        renderer.sprite = card.CardSprite;
    }
}
