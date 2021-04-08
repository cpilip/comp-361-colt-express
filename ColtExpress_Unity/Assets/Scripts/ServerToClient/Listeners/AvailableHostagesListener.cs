using HostageSpace;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvailableHostagesListener : UIEventListenable
{
    public GameObject hostagesList;
    public override void updateElement(string data)
    {
        /* PRE: data 
        *
                {
                    eventName = action,
                    availableHostages = (List<HostageChar>)
                };
        */

        JObject o = JObject.Parse(data);
        List<HostageChar> hostages = o.SelectToken("availableHostages").ToObject<List<HostageChar>>();

        string availableHostages = "";

        foreach (HostageChar h in hostages)
        {
            GameUIManager.gameUIManagerInstance.getHostage(h).SetActive(true);
            availableHostages += (h.ToString() + " ");
        }

        Debug.Log("[AvailableHostagesListener] Hostages available: " + availableHostages + ".");

    }
}
