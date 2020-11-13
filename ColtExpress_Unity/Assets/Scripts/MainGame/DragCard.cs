using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;

public class DragCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
  public void OnBeginDrag(PointerEventData eventdata){

  }
  public void OnDrag(PointerEventData eventData){
    this.transform.position = eventData.position;
  }

  public void OnEndDrag(PointerEventData eventData){
  }

}
