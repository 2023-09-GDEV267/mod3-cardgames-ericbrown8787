using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternateDeckTest : MonoBehaviour
{
    [Header("Set In Inspector")]
    public TextAsset deckXML;
    public Layout layout;
    public TextAsset layoutXML;
    public float xOffset = 3;
    public float yOffset = -2.5f;
    public Vector3 layoutCenter;

    [Header("Set Dynamically")]
    public Deck deck;
    public Transform layoutAnchor;
    public List<Card> drawPile;
    public List<Card> tableau;

    // Start is called before the first frame update
    void Start()
    {
        deck = GetComponent<Deck>();
        deck.InitDeck(deckXML.text);
/*        Card c;*/
        /*        for (int cNum = 0; cNum < deck.cards.Count; cNum++)
                {
                    c = deck.cards[cNum];
                    c.faceUp = true;
                    c.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0f);

                }*/
        layout = GetComponent<Layout>();
        layout.ReadLayout(layoutXML.text);

      /*  LayoutGame();*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
/*
    void LayoutGame()
    {
        // Create an empty gameobject to serve as the anchor for the tableau
        if (layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            // Create an empty gameobject called _LayoutAnchor in the heirarchy
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }
        Card cp;
        // Folow the layout
        foreach (SlotDef tSD in layout.slotDefs)
        {
            // Iterate through all SlotDefs in the layout.slotDefs as tSD
            cp = Draw();
            cp.faceUp = tSD.faceUp;
            cp.transform.parent = layoutAnchor;
            cp.transform.localPosition = new Vector3(
                layout.multiplier.x * tSD.x,
                layout.multiplier.y * tSD.y,
                -tSD.layerID);
            // ^ Set the localPosition of the card based on SlotDef
            cp.layoutID = tSD.id;
            cp.slotDef = tSD;

            // Cards in the tableau have the state CardState.tableau
            cp.state = eCardState.tableau;

            cp.SetSortingLayerName(tSD.layerName);
            tableau.Add(cp); // Add this Card to the List<> tableau

        }

        foreach (Card tCP in tableau)
        {
            foreach (int hid in tCP.slotDef.hiddenBy)
            {
                cp = FindCardByLayoutID(hid);
                tCP.hiddenBy.Add(cp);
            }
        }

        // Set up the initial target card
        MoveToTarget(Draw());
        //Set up the draw pile
        UpdateDrawPile();
    }

    Card Draw()
    {
        Card cd = drawPile[0];
        drawPile.RemoveAt(0);
        return (cd);

    }

    void UpdateDrawPile()
    {
        Card cd;
        // Go through all the cards of the drawpile
        for (int i = 0; i < drawPile.Count; i++)
        {
            cd = drawPile[i];
            cd.transform.parent = layoutAnchor;

            // Position it correctly with the layout.drawPile.stagger
            Vector2 dpStagger = layout.drawPile.stagger;
            cd.transform.localPosition = new Vector3(
                            layout.multiplier.x * layout.drawPile.x,
            layout.multiplier.y * layout.drawPile.y,
            -layout.drawPile.layerID + 0.1f * i);
            cd.faceUp = false; // make them all face-down
            cd.state = eCardState.drawpile;
            //Set dept sorting
            cd.SetSortingLayerName(layout.drawPile.layerName);
            cd.SetSortOrder(-10 * i);
        }
    }*/
}
