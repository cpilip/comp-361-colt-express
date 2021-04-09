using GameUnitSpace;
using HostageSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateHostageNameListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        /* PRE: data 
        *
            {
                eventName = action,
                player = (Character)args[0],
                hostage = (HostageChar)arg[1]
            };
        */

        JObject o = JObject.Parse(data);
        Character c = o.SelectToken("player").ToObject<Character>();
        HostageChar h = o.SelectToken("hostage").ToObject<HostageChar>();

        GameUIManager.gameUIManagerInstance.getPlayerProfileObject(c).transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = h.ToString();

        Debug.Log("[UpdateHostageNameListener] Player: " + c.ToString() + " has " + h.ToString());

    }
}
