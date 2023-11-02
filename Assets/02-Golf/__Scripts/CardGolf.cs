using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCardGolfState
{
    drawpile,
    tableau,
    target,
    discard
}
public class CardGolf : Card 
{
    [Header("Set Dynamically: CardGolf")]
    // This is how you use the enum eCardState
    public eCardGolfState state = eCardGolfState.drawpile;
    // The hiddenby list stores which other cards will keep this one face down
    public List<CardGolf> hiddenBy = new List<CardGolf>();
    // The layoutID matches this card to the tableau XML if it's a tableaux card
    public int layoutID;
    // The SlotDef class stores information pulled in from the LayoutXML <slot>
    public SlotDef slotDef;

    public override void OnMouseUpAsButton()
    {
        // Call the CardClicked method of the Prospector Singleton
        Golf.S.CardClicked(this);
        // Also call the base class version of this method
        base.OnMouseUpAsButton();
    }
}
