using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardTaker : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Transform baseParent;

    private CardManager cardManager;

    private Vector2 basePos;
    // Set the new size of the card when we go over
    private Vector2 newSize = new Vector2(250, 250);
    private Vector2 baseSize;

    private TextMeshProUGUI category;
    private TextMeshProUGUI subcategory;

    private string line;
    private TurnSys turnSys;


    private void Start()
    {
        turnSys = FindObjectOfType<TurnSys>();
        cardManager = GameObject.FindObjectOfType<CardManager>();
        category = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        subcategory = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        // Set base parent
        baseParent = transform.parent;
        basePos = transform.position;
        baseSize = transform.GetComponent<RectTransform>().sizeDelta;
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Set the new size of the card
        transform.GetComponent<RectTransform>().sizeDelta = newSize;

        // Set the cursor invisible
        Cursor.visible = false;
        
        // Set the card on the mouse position
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset the size of the card
        transform.GetComponent<RectTransform>().sizeDelta = baseSize;

        // Set the cursor visible
        Cursor.visible = true;
        
        // Set the position of the card
        transform.position = basePos;


        // Set the UI detector system
        List<RaycastResult> uiUnder = RaycastMouse();

        // Check every case for uiUnder
        if(uiUnder[0].gameObject.tag == "AreaToPlay")
        {
            // PLAYER PLAYS HERE
            Debug.Log("Card has been destroyed.");

            cardManager.PlayAndTurn("");
            

            Destroy(gameObject);
        }
        else if(uiUnder[0].gameObject.tag == "AreaForHand")
        {
            Debug.Log("Card has been replaced in hand.");
        }
        else if(uiUnder[1].gameObject.tag == "Reserve")
        {
            if (uiUnder[1].gameObject.GetComponent<BasicalBoolean>().hasBeenActivated)
            {
                // Disagree the placing in the reserve
                Debug.Log("Card has been replaced in hand because reserve is full.");
            }
            else
            {
                // Agree with placing in reserve
                Debug.Log("Placed in reserve");

                uiUnder[1].gameObject.GetComponent<Image>().sprite = transform.GetComponent<Image>().sprite;

                uiUnder[1].gameObject.GetComponent<DesignedCard>().category.text = transform.GetComponent<DesignedCard>().category.text;
                uiUnder[1].gameObject.GetComponent<DesignedCard>().subcategory.text = transform.GetComponent<DesignedCard>().subcategory.text;

                uiUnder[1].gameObject.GetComponent<BasicalBoolean>().hasBeenActivated = true;

                Destroy(gameObject);
            }
        }
    }

    // Allows to spot things under mouse
    public List<RaycastResult> RaycastMouse()
    {

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };

        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        Debug.Log(results[0].gameObject.name);

        return results;
    }
}
