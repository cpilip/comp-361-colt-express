using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
  public Transform parentToReturnTo;

    public void OnBeginDrag(PointerEventData eventdata)
  {
        // Remember the card's parent panel (the subdeck)
        parentToReturnTo = this.transform.parent;
        // Set the card's parent to the overall deck
        this.transform.SetParent(this.transform.parent.parent);

        // Disable raycasting on card so onDrop() will work
        GetComponent<CanvasGroup>().blocksRaycasts = false;
  }
  public void OnDrag(PointerEventData eventData)
  {
        // Card follows mouse
        this.transform.position = Input.mousePosition;

    }


    public void OnEndDrag(PointerEventData eventData)
  {
        this.transform.SetParent(parentToReturnTo);

        // Re-enable raycasting 
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    

}
