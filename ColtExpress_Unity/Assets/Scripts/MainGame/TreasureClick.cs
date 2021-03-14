using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TreasureClick : MonoBehaviour, IPointerDownHandler
{
    // Update is called once per frame
    public GameObject hidePanel;

    public void OnPointerDown(PointerEventData eventData)
    {
            Debug.Log(this.gameObject.name + " Was Clicked.");
            this.gameObject.SetActive(false);
            hidePanel.SetActive(true);
    }
}
