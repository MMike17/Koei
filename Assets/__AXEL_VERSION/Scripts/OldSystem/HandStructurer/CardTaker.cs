using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardTaker : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Transform baseParent;

    private Vector2 basePos;
    // Set the new size of the card when we go over
    private Vector2 newSize = new Vector2(250, 250);
    private Vector2 baseSize;


    private void Start()
    {
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
            Debug.Log("Card has been destroyed.");
            Destroy(gameObject);
        }
        else if(uiUnder[0].gameObject.tag == "AreaForHand")
        {
            Debug.Log("Card has been replaced in hand.");
        }
        else
        {
            Debug.Log("Do not detect the UI.");
        }

    }

    public List<RaycastResult> RaycastMouse()
    {

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };

        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        Debug.Log(results.Count);

        return results;
    }
}
