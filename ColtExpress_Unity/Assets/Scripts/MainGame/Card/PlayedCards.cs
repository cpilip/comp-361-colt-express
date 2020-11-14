using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;

public class PlayedCards : MonoBehaviour, IDropHandler
{

   
    public void OnDrop(PointerEventData eventData)
    {
        // Debug.Log(" dropped on " + gameObject.name);
        

        DragCard d = eventData.pointerDrag.GetComponent<DragCard>();

        // Add the card to the played cards zone
        // No further functionality currently, just that you're able to do so
        if (d != null)
        {
            d.parentToReturnTo = this.transform;
        }
    }
}
