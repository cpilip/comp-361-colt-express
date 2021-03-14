﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

public class TextElementListener : UIEventListenable
{
    
    public override void updateElement()
    {
        this.GetComponent<TextMeshProUGUI>().text = 3.ToString();
        Debug.Log("Updated text element of listener " + this);
    }
}
