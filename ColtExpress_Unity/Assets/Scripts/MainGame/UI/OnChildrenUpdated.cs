using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* Author: Christina Pilip
 * Usage: Defines behaviour when a Unity object's children are changed. 
 */
public class OnChildrenUpdated : MonoBehaviour
{
    public bool hasChanged;
    private int previousNumChildren;

    void Start()
    {
        hasChanged = false;
        previousNumChildren = this.transform.childCount;
    }

    public void OnTransformChildrenChanged()
    {
        Debug.Log("The number of children was changed.");
        if (previousNumChildren != this.transform.childCount)
        {
            previousNumChildren = this.transform.childCount;
            hasChanged = true;
        }
        else
        {
            hasChanged = false;
        }

    }
}
