using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Card : MonoBehaviour {

	public string suit;
	public int rank;
	public Color color = Color.black;
	public string colS = "Black";  // or "Red"

	public List<GameObject> decoGOs = new List<GameObject>();
	public List<GameObject> pipGOs = new List<GameObject>();

	public GameObject back;  // back of card;
	public CardDefinition def;  // from DeckXML.xml		

	// List of the SpriteRenderer Components of this GameObject and its children
	public SpriteRenderer[] spriteRenderers;




	// Use this for initialization
	void Start() {
		SetSortOrder(0); //Ensures this card starts properly depth sorted
	}

	// Update is called once per frame
	void Update() {

	}
	// If SpriteRenderers is not already defined, this function defines it
	public void PopulateSpriteRenderers()
	{
		//if spriteRenderers is null or empty
		if (spriteRenderers == null || spriteRenderers.Length == 0)
		{
			// Get SpriteRenderer Components of this GameObject and its children
			spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
		}
	}

	// Sets the sortingLayerName in all SpriteRenderer Components
	public void SetSortingLayerName(string tSLN)
	{
		PopulateSpriteRenderers();
		foreach (SpriteRenderer tSR in spriteRenderers)
		{
			tSR.sortingLayerName = tSLN;
		}
	}

	public void SetSortOrder(int sOrd)
	{
		PopulateSpriteRenderers();
		// Iterate through all the spriteRenderers as tSR
		foreach (SpriteRenderer tSR in spriteRenderers)
		{
			if (tSR.gameObject == this.gameObject)
			{
				// If the gameObject is this.gameObject, it's the background
				tSR.sortingOrder = sOrd;
				continue;

			}
			//Each of the children of this gameobject are named
			//switch based on the name
			switch (tSR.gameObject.name) 
			{
				case "back": // if the naem is "back"
							 //set it to the highest layer to cover other sprites
					tSR.sortingOrder = sOrd + 2;
					break;

				case "face": //If the name is "face" 
				default: // Or if it's anything else
						 // Set it to the middle layer to be above the background
					tSR.sortingOrder = sOrd + 1;
					break;
			}
		}
	}

    public bool faceUp
    {
        get
        {
            return (!back.activeSelf);
        }

        set
        {
            back.SetActive(!value);
        }
    }

	// Virtual methods can be overridden by subclass methods with the same name
	virtual public void OnMouseUpAsButton()
	{

		//Commenting this out so I can actually find my other prints
/*		print(name); // When clicked, print the card name to the console*/
	}


} // class Card

[System.Serializable]
public class Decorator{
	public string	type;			// For card pips, tyhpe = "pip"
	public Vector3	loc;			// location of sprite on the card
	public bool		flip = false;	//whether to flip vertically
	public float 	scale = 1.0f;
}

[System.Serializable]
public class CardDefinition{
	public string	face;	//sprite to use for face cart
	public int		rank;	// value from 1-13 (Ace-King)
	public List<Decorator>	
					pips = new List<Decorator>();  // Pips Used
}
