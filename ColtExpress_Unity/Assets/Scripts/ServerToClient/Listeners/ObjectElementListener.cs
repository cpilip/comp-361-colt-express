using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;


public class ObjectElementListener : UIEventListenable
{

    public override void updateElement(String data)
    {
        JSONTestObject deserializedProduct = JsonConvert.DeserializeObject<JSONTestObject>(data);

        Debug.Log("Testing JSON " + deserializedProduct.Name + " " + deserializedProduct.Value);
    }
}
