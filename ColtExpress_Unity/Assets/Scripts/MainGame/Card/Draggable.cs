using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;

/* Author: Christina Pilip
 * Usage: Defines a property "Draggable", allowing a Unity object to be dragged.
 */
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentToReturnTo;
    public int originalIndex;
    public bool fromDeck = true;

    public void OnBeginDrag(PointerEventData eventdata)
    {
        // Retrieve the Draggable's sibling index
        originalIndex = this.transform.GetSiblingIndex();

        // Remember the Draggable's parent panel
        parentToReturnTo = this.transform.parent;
        // Set the Draggable's parent to the "master" parent
        this.transform.SetParent(this.transform.parent.parent);

        // Disable raycasting on Draggable so onDrop() will work
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        // Draggable follows mouse
        this.transform.position = Input.mousePosition;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // If not dropped on a Drop Zone, return the Draggable to the parent panel
        this.transform.SetParent(parentToReturnTo);


        if (parentToReturnTo != null)
        {
            if (parentToReturnTo.name == "Played Cards")
            {
                this.transform.SetSiblingIndex(parentToReturnTo.childCount - 1);

                Debug.LogError("IN " + parentToReturnTo + " - INDEX " + this.transform.GetSiblingIndex());
            } else
            {

                this.transform.SetSiblingIndex(originalIndex);
                Debug.LogError("IN " + parentToReturnTo + " - INDEX " + this.transform.GetSiblingIndex());
            }
        } 

        // Re-enable raycasting 
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

}
