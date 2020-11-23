using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnChildrenUpdated : MonoBehaviour
{
    public GameObject handBlocker;
    public void OnTransformChildrenChanged()
    {
        if (handBlocker != null)
        {
            handBlocker.SetActive(!handBlocker.activeSelf);
        }
        //TODO: Update subdecks

        //Debug.Log("A child was changed for: " + this.GetComponentInParent<Transform>().gameObject.name);
    }
}
