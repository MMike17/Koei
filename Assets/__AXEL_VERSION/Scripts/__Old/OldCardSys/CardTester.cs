// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using CardSystem;

// public class CardTester : MonoBehaviour
// {
//     public List<Card> totalCards;

//     // Set the hand
//     public Transform handObject;
//     List<Transform> handChildren;

//     // Start is called before the first frame update
//     void Start()
//     {
//         CardSys.GenerateDeckFullRandom(5, totalCards);

//         handChildren = new List<Transform>();
//         foreach(Transform child in handObject)
//         {
//             handChildren.Add(child);
//         }

//         for (int i = 0; i < handChildren.Count; i++)
//         {
//             handChildren[i].GetComponent<DesignedCard>();

//             handChildren[i].GetComponent<DesignedCard>().Init(totalCards[i]);
//             /*
//             handChildren[i].GetComponent<DesignedCard>().category.text = totalCards[i].strength.ToString();
//             handChildren[i].GetComponent<DesignedCard>().subcategory.text = totalCards[i].subStrength.ToString();
//             */
//         }
//     }
// }
