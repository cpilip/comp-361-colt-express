using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* Author: Christina Pilip
 * Usage: Defines behaviour when a Unity object's children are changed. 
 */
public class OnChildrenUpdated : MonoBehaviour
{
    public delegate void wasChildChanged();
    public event wasChildChanged notifyChildWasChanged;

    public void OnTransformChildrenChanged()
    {
        notifyChildWasChanged?.Invoke();
    }
    

}
