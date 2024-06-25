using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGame : MonoBehaviour
{
    private Deck deck;
    private List<GameObject> instantiatedCards = new List<GameObject>();

    public GameObject cardPrefab;
    public GameObject hand;

    void Start()
    { 
        NewGame();
    }

    public void NewGame()
    {
        foreach(GameObject card in instantiatedCards)
        {
            Destroy(card);
        }
        instantiatedCards.Clear();

        deck = new Deck();
        deck.Shuffle();
        Debug.Log("Shuffled Deck:");
        DisplayDeck();
        Debug.Log("Dealing Cards:");
        for (int i = 0; i < 5; i++)
        {
            Card dealtCard = deck.Deal();
            Debug.Log(dealtCard);
            DisplayCard(dealtCard, i);
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
    private void DisplayCard(Card card, int index)
    {
        Vector3 position = hand.transform.position + new Vector3(index * 2.0f, 0, 0);
        GameObject cardObject = Instantiate(cardPrefab, position, Quaternion.identity, hand.transform);
        SpriteRenderer renderer = cardObject.GetComponent<SpriteRenderer>();
        renderer.sprite = card.CardSprite;
        instantiatedCards.Add(cardObject);
    }
}
