using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

/* Author: Christina Pilip
 * Usage: Defines behavior for mousing over a tooltip.
 */
public class MouseOverTooltipDisplay : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public GameObject tooltip;
    public void OnPointerEnter(PointerEventData eventdata)
    {
        if (tooltip != null)
        {
            // Display the tooltip
            tooltip.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventdata)
    {
        if (tooltip != null)
        {
            tooltip.SetActive(false);
        }
    }
}
