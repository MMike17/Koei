using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AiElementsDefinitions;

public class TurnSys : MonoBehaviour
{
    public Entity.EntityGenre entityToPlay;
    
    public void EndTurn()
    {
        if(entityToPlay == Entity.EntityGenre.Ai)
        {
            entityToPlay = Entity.EntityGenre.Player;
        }
        else if(entityToPlay == Entity.EntityGenre.Player)
        {
            entityToPlay = Entity.EntityGenre.Ai;
        }
    }
}
