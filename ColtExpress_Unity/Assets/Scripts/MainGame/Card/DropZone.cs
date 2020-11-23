using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;

/* Author: Christina Pilip
 * Usage: Defines a property "Drop Zone", allowing a "Draggable" to be parented to the Drop Zone.
 */
public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();

        // Add the Draggable to the Drop Zone
        if (d != null)
        {
            d.parentToReturnTo = this.transform;
        }
    }
}
