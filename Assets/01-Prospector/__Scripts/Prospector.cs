using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Prospector : MonoBehaviour {

	static public Prospector 	S;

	[Header("Set in Inspector")]
	public TextAsset			deckXML;
	public TextAsset layoutXML;
	public float xOffset = 3;
	public float yOffset = -2.5f;
	public Vector3 layoutCenter;


	[Header("Set Dynamically")]
	public Deck					deck;
	public Layout layout;
	public List<CardProspector> drawPile;
	public Transform layoutAnchor;
	public CardProspector target;
	public List<CardProspector> tableau;
	public List<CardProspector> discardPile;

	void Awake(){
		S = this;
	}

	void Start() {
		deck = GetComponent<Deck> ();
		deck.InitDeck (deckXML.text);
		Deck.Shuffle(ref deck.cards);
/*		Card c;
		for (int cNum = 0; cNum < deck.cards.Count; cNum++)
		{
			c = deck.cards[cNum];
			
			c.transform.localPosition = new Vector3((cNum%13)*3, cNum/13*4,0f);

		}*/
	
		layout = GetComponent<Layout> ();
		layout.ReadLayout(layoutXML.text);
		drawPile = ConvertListCardsToListCardProspectors(deck.cards);
		LayoutGame();
	}
	CardProspector Draw()
	{
		CardProspector cd = drawPile[0];
		drawPile.RemoveAt(0);
		return (cd);

	}
	void LayoutGame()
	{
		// Create an empty gameobject to serve as the anchor for the tableau
		if (layoutAnchor== null)
		{
			GameObject tGO = new GameObject("_LayoutAnchor");
			// Create an empty gameobject called _LayoutAnchor in the heirarchy
			layoutAnchor = tGO.transform;
			layoutAnchor.transform.position = layoutCenter;
		}
		CardProspector cp; 
		// Folow the layout
		foreach(SlotDef tSD in layout.slotDefs)
		{
			// Iterate through all SlotDefs in the layout.slotDefs as tSD
			cp = Draw();
			cp.faceUp= tSD.faceUp;
			cp.transform.parent = layoutAnchor;
			cp.transform.localPosition = new Vector3(
				layout.multiplier.x * tSD.x,
                layout.multiplier.y * tSD.y,
				-tSD.layerID);
			// ^ Set the localPosition of the card based on SlotDef
			cp.layoutID = tSD.id;
			cp.slotDef= tSD;

			// CardProspectors in the tableau have the state CardState.tableau
			cp.state = eCardState.tableau;

			cp.SetSortingLayerName(tSD.layerName);
			tableau.Add(cp); // Add this CardProspector to the List<> tableau

		}
	}

	List<CardProspector> ConvertListCardsToListCardProspectors(List<Card> lCD)
	{
		List<CardProspector> lCP = new List<CardProspector>();
		CardProspector tCP;
		foreach(Card tCD in lCD) {
		tCP = tCD as CardProspector;	
		lCP.Add(tCP);
		}
		return lCP;
	}

}
