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

		foreach (CardProspector tCP in tableau)
		{
			foreach(int hid in tCP.slotDef.hiddenBy)
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

	CardProspector FindCardByLayoutID(int layoutID)
	{
		foreach (CardProspector tCP in tableau)
		{

			// Search through all cards in the tableau list<>
			if (tCP.layoutID == layoutID) return tCP;
			
		}
        return (null);
    }

	// This turns cards in the mine face up or face down
	void SetTableauFaces()
	{
		foreach (CardProspector cd in tableau)
		{
			bool faceUp = true; // Assumes the card will be face up
			foreach(CardProspector cover in cd.hiddenBy)
			{
				// If either of the covering cards are in the tableau
				if (cover.state == eCardState.tableau)
				{
					faceUp = false;
				}
			}
			cd.faceUp = faceUp; //Set the value on the card
		}
	}

	void MoveToDiscard(CardProspector cd)
	{
		// Set the state of the card to discard
		cd.state = eCardState.discard;
		discardPile.Add(cd); // Add it to the DiscardPile list
		cd.transform.parent = layoutAnchor; //Update its transform parent

		// Position this card in the discard pile
		cd.transform.localPosition = new Vector3(
			layout.multiplier.x * layout.discardPile.x,
			layout.multiplier.y * layout.discardPile.y,
			-layout.discardPile.layerID + 0.5f);

		cd.faceUp = true;
		// Place it on top of the pile for sorting
		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(-100 + discardPile.Count);
	}

	void MoveToTarget(CardProspector cd)
	{
		// If there is currently a target card, move it to the discard pile
		if (target!= null) MoveToDiscard(target);
		target= cd; // cd is the new target
		cd.state = eCardState.target;
		cd.transform.parent = layoutAnchor;

		//Move to the target position
		cd.transform.localPosition = new Vector3(
			layout.multiplier.x * layout.discardPile.x,
			layout.multiplier.y * layout.discardPile.y,
			-layout.discardPile.layerID);

		cd.faceUp = true; // Make it face-up
						  // Set the depth sorting
		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(0);
	}

	// Arranges all cards on the draw pile to show how many are left

	void UpdateDrawPile()
	{
		CardProspector cd;
		// Go through all the cards of the drawpile
		for(int i=0; i<drawPile.Count; i++)
		{
			cd = drawPile[i];
			cd.transform.parent= layoutAnchor;

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
			cd.SetSortOrder(-10*i);
		}
	}

	public void CardClicked(CardProspector cd)
	{
		switch(cd.state)
		{
			case eCardState.target:
				break;

			case eCardState.drawpile:
				// Clicking any card in the drawpile will draw the next card
				MoveToDiscard(target); //Moves the target to the discard pile
				MoveToTarget(Draw());// Moves the next drawn card to the target
				UpdateDrawPile(); // Restacks the drawpile
				ScoreManager.EVENT(eScoreEvent.draw);
				break;

			case eCardState.tableau:
				// Clicking a card in the tableau will check if it's a valid play
				bool validMatch = true;
				if (!cd.faceUp)
				{
					// If the card is face down, it's not valid
					validMatch= false;
				}
				if (!AdjacentRank(cd, target))
				{
					// If it's not an adjacent rank, it's not valid
					validMatch = false;	
				}
				if (!validMatch)
				{
					return;// return if not valid
				}
				// If we got here, it's a valid card
				tableau.Remove(cd);
				MoveToTarget(cd);
				SetTableauFaces();
                ScoreManager.EVENT(eScoreEvent.mine);
                break;	
		}
		// Check to see whether game is over or not
		CheckForGameOver();
	}

	void CheckForGameOver()
	{
		// If the tableau is empty, the game is over
		if (tableau.Count == 0)
		{
			GameOver(true);
			return;
		}

		// If there are still cards in the draw pile, the game's not over
		if (drawPile.Count > 0)
		{
			return;
		}

		// Check for remaining valid plays
		foreach (CardProspector cd in tableau)
		{
			if (AdjacentRank(cd, target))
			{
				// If there is still a valid play, the game's not over
				return;
			}
		}

		// Since there are no valid plays, the game is over
		// Call GameOver with a loss
		GameOver(false);
	}

	void GameOver(bool won) {
	if (won)
		{
/*			print("Game over. You won! :)");*/
            ScoreManager.EVENT(eScoreEvent.gameWin);
        }
        else
		{
/*			print("Game over. You lost. :(");*/
            ScoreManager.EVENT(eScoreEvent.draw);
        }
		SceneManager.LoadScene("__Prospector");
	}

	public bool AdjacentRank(CardProspector c0, Card c1) {
		// If either card is face-down, it's not adjacent
		if (!c0.faceUp||!c1.faceUp) return false;

		// If they are 1 apart they are adjacent
		if (Mathf.Abs(c0.rank - c1.rank) == 1)
		{
			return true;
		}

		// If one is Ace and the other is king, they are adjacent
		if (c0.rank == 1 && c1.rank == 13) 
		{
			return true;
		}

		if (c0.rank == 13 && c1.rank == 1)
		{
			return true;
		}

		return false;
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
