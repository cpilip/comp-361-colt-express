using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* Author: Christina Pilip
 * Usage: Defines behaviour when a Unity object's children are changed. 
 */
public class OnChildrenUpdated : MonoBehaviour
{
    public GameObject handBlocker;
    public void OnTransformChildrenChanged()
    {
        if (handBlocker != null)
        {
            handBlocker.SetActive(!handBlocker.activeSelf);
        }
        //TODO: Figure out how to update subdecks (see Phase1Action)

        //Debug.Log("A child was changed for: " + this.GetComponentInParent<Transform>().gameObject.name);
    }
}
