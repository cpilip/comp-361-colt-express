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
        System.Object[] deserializedProduct = JsonConvert.DeserializeObject<System.Object[]>(data, CommunicationAPIHandler.settings);

        Debug.Log(deserializedProduct.Length);

        Debug.Log(deserializedProduct[0].GetType());


    }
}
