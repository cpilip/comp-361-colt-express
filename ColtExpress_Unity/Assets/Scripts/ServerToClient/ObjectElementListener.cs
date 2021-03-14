using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObjectElementListener : UIEventListenable
{

    public override void updateElement()
    {
        Debug.Log("Updated object element of listener " + this);
    }
}
