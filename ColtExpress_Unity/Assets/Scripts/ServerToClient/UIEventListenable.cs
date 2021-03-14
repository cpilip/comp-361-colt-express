using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIEventListenable : MonoBehaviour
{
    public String eventName;

    void Start()
    {
        EventManager.StartListening(eventName, updateElement);
    }

    void OnDisable()
    {
        EventManager.StopListening(eventName, updateElement);
    }

    public abstract void updateElement();
}
