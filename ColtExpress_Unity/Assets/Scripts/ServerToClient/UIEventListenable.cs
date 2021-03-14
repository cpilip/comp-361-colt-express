using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Christina Pilip
 * Usage: An abstract class that can be extended to create a variety of listener UI elements.
 * 
 * 1. Extend this class in a new EventListener class.
 * 2. Place a copy of that script on the object you want to be a listener.
 * 3. Override updateElement() with the appropriate behaviour.
 * 4. Give the event a name in the editor.
 * 5. To trigger the event and the corresponding behaviour, call: EventManager.TriggerEvent(yourEventName);
 */

public abstract class UIEventListenable : MonoBehaviour
{
    //Event name string
    public String eventName;

    void Start()
    {
        EventManager.StartListening(eventName, updateElement);
    }

    void OnDisable()
    {
        EventManager.StopListening(eventName, updateElement);
    }

    public abstract void updateElement(string data);
}
