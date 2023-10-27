using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternateDeckTest : MonoBehaviour
{
    [Header("Set In Inspector")]
    public TextAsset deckXML;

    [Header("Set Dynamically")]
    public Deck deck;
    
    // Start is called before the first frame update
    void Start()
    {
        deck = GetComponent<Deck>();
        deck.InitDeck(deckXML.text);
        Card c;
        for (int cNum = 0; cNum < deck.cards.Count; cNum++)
        {
            c = deck.cards[cNum];
            c.faceUp = true;
            c.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0f);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
