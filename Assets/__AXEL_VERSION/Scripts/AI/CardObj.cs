using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObj
{
    public string id;
    public float probabilityToUse;
    public Card card;

    public CardObj(Card aiCard, float probability)
    {
        id = aiCard.name;
        probabilityToUse = probability;
        card = aiCard;
    }
}
