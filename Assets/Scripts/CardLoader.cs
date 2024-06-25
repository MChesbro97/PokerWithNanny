using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLoader : MonoBehaviour
{
    public string cardName;  // Example: "2_of_Hearts"
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        LoadCardSprite(cardName);
    }

    void LoadCardSprite(string cardName)
    {
        string path = $"PlayingCards/{cardName}";
        Sprite cardSprite = Resources.Load<Sprite>(path);

        if (cardSprite != null)
        {
            spriteRenderer.sprite = cardSprite;
        }
        else
        {
            Debug.LogError($"Card sprite not found at path: {path}");
        }
    }
}
