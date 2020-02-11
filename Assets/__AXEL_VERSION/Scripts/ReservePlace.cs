using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReservePlace : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public GameObject reserve;

    private BasicalBoolean boolean;
    private Transform parents;
    private Vector2 posi;

    private RectTransform rect;

    private void Start()
    {
        Debug.Log("Has just spawned");

        rect = gameObject.GetComponent<RectTransform>();
        posi = gameObject.GetComponent<RectTransform>().position;
        parents = transform.parent;

        boolean = GetComponent<BasicalBoolean>();
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (boolean.hasBeenActivated)
        {
            Debug.Log("Activated card and is going to be played");

            // Set the cursor invisible
            Cursor.visible = false;

            // Set the card on the mouse position
            transform.position = Input.mousePosition;
        }
        else
        {
            Debug.Log("Nothing was activated in reserve");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Has calculated the end of the drag");

        List<RaycastResult> uiUnder = RaycastMouse();

        if (boolean.hasBeenActivated)
        {
            Debug.Log("Has been calculated the place ; The number of objects under pointer is " + uiUnder.Count);

            // On placing card on 'AreaToPlay' place
            if (uiUnder[0].gameObject.tag == "AreaToPlay")
            {
                Debug.Log("Successful moment ! Your card just was played now !");

                GameObject newReserve = Instantiate(reserve, parents);                  // ERROR HERE
                newReserve.GetComponent<RectTransform>().position = posi;
                newReserve.GetComponent<RectTransform>().sizeDelta = rect.sizeDelta;    

                Cursor.visible = true;

                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Wrong place for reserve, replacing reserve at the good place");
                Cursor.visible = true;
                GetComponent<RectTransform>().position = rect.position;
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

        return results;
    }
}
