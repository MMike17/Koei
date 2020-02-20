using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AiCore;
using AiLib;
using AiElementsDefinitions;

public class CardAi : MonoBehaviour
{
    private void Start()
    {
        // Generate new AI
        Ai newAi = Ai.CreateNewAi();
    }
}
